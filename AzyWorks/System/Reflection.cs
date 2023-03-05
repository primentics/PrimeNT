using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using AzyWorks.Extensions;
using AzyWorks.System.Values;

namespace AzyWorks.System
{
    public static class Reflection
    {
        public static readonly IReadOnlyList<TypeCode> PrimitiveTypes = new List<TypeCode>() 
                                                                        { TypeCode.Boolean, TypeCode.Byte, TypeCode.SByte, TypeCode.Int16,
                                                                          TypeCode.UInt16, TypeCode.Int32, TypeCode.UInt32, TypeCode.Int64,
                                                                          TypeCode.UInt64, TypeCode.Single, TypeCode.Double, TypeCode.Decimal,
                                                                          TypeCode.DateTime, TypeCode.Char, TypeCode.String };

        public static T InstantiateWithGeneric<T>(Type genericType)
        {
            return As<T>(Instantiate(typeof(T).MakeGenericType(genericType)));
        }

        public static T InstantiateWithGeneric<T>(Type type, Type genericType)
        {
            return As<T>(Instantiate(type.MakeGenericType(genericType)));
        }

        public static T InstantiateAs<T>()
        {
            return Activator.CreateInstance<T>();
        }

        public static T InstantiateAs<T>(Type type, params object[] args)
        {
            var typeMap = args.Select(x => x.GetType()).ToArray();
            var constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic);

            ConstructorInfo selectedConstructor = null;
            foreach (var constructor in constructors)
            {
                var parameters = constructor.GetParameters();
                var parameterTypes = parameters.Select(x => x.ParameterType);

                if (!parameterTypes.Match(typeMap))
                    continue;

                selectedConstructor = constructor;
                break;
            }

            if (selectedConstructor is null)
                return default;

            var instance = selectedConstructor.Invoke(args);

            return As<T>(instance);
        }

        public static T InstantiateAs<T>(params object[] args)
        {
            var type = typeof(T);
            var typeMap = args.Select(x => x.GetType()).ToArray();
            var constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic);

            ConstructorInfo selectedConstructor = null;
            foreach (var constructor in constructors)
            {
                var parameters = constructor.GetParameters();

                if (parameters.Length != typeMap.Length)
                    continue;

                for (int i = 0; i < typeMap.Length;)
                {
                    if (i + 1 >= typeMap.Length && typeMap[i] == parameters[i].ParameterType)
                    {
                        selectedConstructor = constructor;
                        break;
                    }

                    if (typeMap[i] != parameters[i].ParameterType)
                        i++;
                }

                if (selectedConstructor != null)
                    break;
            }

            var instance = selectedConstructor.Invoke(args);

            return SafeAs<T>(instance);
        }

        public static void Execute(Type type, string methodName, object handle = null, params object[] parameters)
        {
            var method = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic);

            if (method is null)
                throw new MissingMethodException(methodName);

            method.Invoke(handle, parameters);
        }

        public static void Execute<T>(string methodName, T handle = default, params object[] parameters)
            => Execute(typeof(T), methodName, handle, parameters);

        public static T ExecuteReturn<T>(Type type, string methodName, object handle = null, params object[] parameters)
        {
            var method = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic);

            if (method is null)
                throw new MissingMethodException(methodName);

            var res = method.Invoke(handle, parameters);

            if (res is null)
                return default;

            if (!(res is T t))
                return default;

            return t;
        }

        public static TValue ExecuteReturn<TType, TValue>(string methodName, TType handle = default, params object[] parameters)
            => ExecuteReturn<TValue>(typeof(TType), methodName, handle, parameters);

        public static void SetFieldValue<T>(string fieldName, object value, Optional<T> handle = null)
        {
            Optional<T>.Ensure(ref handle);

            var type = typeof(T);

            if (type is null)
                throw new TypeAccessException(nameof(type));

            var field = type.GetField(fieldName);

            if (field is null)
                throw new MissingFieldException(nameof(fieldName));

            if (!field.IsStatic && !handle.HasValue)
                throw new ArgumentNullException(nameof(handle));

            field.SetValue(handle.Value, value);
        }

        public static void SetPropertyValue<T>(string propertyName, object value, Optional<T> handle = null)
        {
            Optional<T>.Ensure(ref handle);

            var type = typeof(T);

            if (type is null)
                throw new ArgumentNullException(nameof(type));

            var property = type.GetProperty(propertyName);

            if (property is null)
                throw new MissingMemberException(nameof(propertyName));

            if (property.SetMethod is null)
                throw new MissingMethodException(nameof(property.SetMethod));

            if (!property.SetMethod.IsStatic && !handle.HasValue)
                throw new ArgumentNullException(nameof(handle));

            property.SetValue(handle.Value, value);
        }

        public static T GetFieldValue<T>(Type type, string fieldName, object handle = null)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            var field = type.GetField(fieldName);

            if (field is null)
                throw new MissingFieldException(nameof(fieldName));

            if (!field.IsStatic && handle is null)
                throw new ArgumentNullException(nameof(handle));

            var value = field.GetValue(handle);

            return As<T>(value);
        }

        public static T GetPropertyValue<T>(Type type, string propertyName, object handle = null)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            var property = type.GetProperty(propertyName);

            if (property is null)
                throw new MissingMemberException(nameof(propertyName));

            if (property.GetMethod is null)
                throw new MissingMethodException(nameof(property.GetMethod));

            if (!property.GetMethod.IsStatic && handle is null)
                throw new ArgumentNullException(nameof(handle));

            var value = property.GetValue(handle);

            return As<T>(value);
        }

        public static T Instantiate<T>(Type type)
        {
            return SafeAs<T>(Instantiate(type));
        }

        public static object Instantiate(Type type)
        {
            return Activator.CreateInstance(type);
        }

        public static T SafeAs<T>(this object obj)
        {
            if (!Is<T>(obj, out var value))
                return default;

            return value;
        }

        public static T As<T>(this object obj)
            => (T)obj;

        public static bool Is<T>(this object obj)
            => obj is T;

        public static bool Is<T>(this object obj, out T t)
        {
            if (obj is T value)
            {
                t = value;
                return true;
            }

            t = default;
            return false;
        }

        public static bool TryGetAttribute<T>(MemberInfo member, out T attributeValue)
        {
            var attributes = member.GetCustomAttributes();

            if (!attributes.Any())
            {
                attributeValue = default;
                return false;
            }

            foreach (var attribute in attributes)
            {
                if (Is(attribute, out attributeValue))
                    return true;
            }

            attributeValue = default;
            return false;
        }

        public static T GetAttribute<T>(MemberInfo member)
        {
            if (!TryGetAttribute<T>(member, out var attribute))
                return default;

            return attribute;
        }

        public static bool HasInterface<T>(Type type, bool checkBaseForInterfaces = false)
            => HasInterface(typeof(T), type, checkBaseForInterfaces);

        public static bool HasInterface(Type interfaceType, Type type, bool checkBaseForInterfaces = false)
        {
            var interfaces = type.GetInterfaces();

            if (interfaces.Length > 0)
            {
                for (int i = 0; i < interfaces.Length; i++)
                {
                    if (interfaces[i] == interfaceType)
                        return true;

                    if (checkBaseForInterfaces)
                    {
                        var baseInterfaces = interfaces[i].GetInterfaces();

                        while (baseInterfaces != null && baseInterfaces.Length > 0)
                        {
                            for (int x = 0; x < baseInterfaces.Length; x++)
                            {
                                if (baseInterfaces[x] == interfaceType)
                                    return true;
                                else
                                    baseInterfaces = baseInterfaces[x].GetInterfaces();
                            }
                        }
                    }
                }
            }
            else if (type.BaseType != null && checkBaseForInterfaces)
            {
                return HasInterface(interfaceType, type.BaseType, checkBaseForInterfaces);
            }

            return false;
        }

        public static bool HasType<T>(Type searchType, bool checkBaseForInterfaces = false)
            => HasType(typeof(T), searchType, checkBaseForInterfaces);

        public static bool HasType(Type type, Type searchType, bool checkBaseForInterfaces = false)
        {
            if (type.BaseType != null)
            {
                if (type.BaseType == searchType)
                    return true;

                if (checkBaseForInterfaces)
                {
                    var baseType = type.BaseType;

                    while (baseType != null)
                    {
                        if (baseType == searchType)
                            return true;
                        else
                            baseType = baseType.BaseType;
                    }
                }
            }

            return false;
        }

        public static bool IsPrimitive(this Type type)
            => PrimitiveTypes.Contains(Type.GetTypeCode(type));

        public static bool IsEnumerable(this Type type)
            => HasInterface(type, typeof(IEnumerable), true);

        public static bool IsDictionary(this Type type)
            => HasInterface(type, typeof(IDictionary), true);
    }
}