using EnumsNET;

using System;
using System.Linq;

namespace AzyWorks.Utilities
{
    public class EnumBuilder<T> where T : struct, Enum
    {
        private T value;

        public EnumBuilder()
        {
            value = default(T);
        }

        public EnumBuilder(T value)
        {
            this.value = value;
        }

        public EnumBuilder<T> WithValue(T value)
        {
            this.value = FlagEnums.CombineFlags<T>(this.value, value);

            if (!FlagEnums.IsValidFlagCombination<T>(this.value))
                throw new InvalidOperationException("Invalid flag combination detected.");

            return this;
        }

        public EnumBuilder<T> WithoutValue(T value)
        {
            this.value = FlagEnums.RemoveFlags<T>(this.value, value);

            if (!FlagEnums.IsValidFlagCombination<T>(this.value))
                throw new InvalidOperationException("Invalid flag combination detected.");

            return this;
        }

        public bool HasValue(T value)
        {
            return GetFlags().Contains(value);
        }

        public T[] GetFlags()
        {
            return FlagEnums.GetFlagMembers<T>(value).Select(x => x.Value).ToArray();
        }

        public T GetValue()
        {
            return value;
        }
    }
}