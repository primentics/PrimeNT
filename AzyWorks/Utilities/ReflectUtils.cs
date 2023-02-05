using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace AzyWorks.Utilities
{
    public static class ReflectUtils
    {
        public const TypeCode NullTypeCode = TypeCode.Empty;

        public static readonly IReadOnlyList<TypeCode> PrimitiveTypes = new List<TypeCode>() 
                                                                        { TypeCode.Boolean, TypeCode.Byte, TypeCode.SByte, TypeCode.Int16,
                                                                          TypeCode.UInt16, TypeCode.Int32, TypeCode.UInt32, TypeCode.Int64,
                                                                          TypeCode.UInt64, TypeCode.Single, TypeCode.Double, TypeCode.Decimal,
                                                                          TypeCode.DateTime, TypeCode.Char, TypeCode.String };

        public static readonly Type VoidType = typeof(void);
        public static readonly Type ObjectType = typeof(object);
        public static readonly Type BoolType = typeof(bool);
        public static readonly Type StringType = typeof(string);

        public static T Instantiate<T>()
        {
            return Activator.CreateInstance<T>();
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