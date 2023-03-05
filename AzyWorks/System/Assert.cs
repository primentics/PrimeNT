using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.CompilerServices;

using AzyWorks.System.Exceptions;

using UnitsNet;

namespace AzyWorks.System
{
    public static class Assert
    {
        public static void True(Func<bool> condition, [CallerMemberName] string memberName = "")
        {
            NotNull(condition);

            if (!condition())
            {
                throw new AssertionFailedException(memberName, "True");
            }
        }

        public static void False(Func<bool> condition, [CallerMemberName] string memberName = "")
        {
            NotNull(condition);

            if (condition())
            {
                throw new AssertionFailedException(memberName, "True");
            }
        }

        public static void Sum<T>(IEnumerable<T> values, Func<T, int> selector, int sum, [CallerMemberName] string memberName = "")
        {
            NotNull(values);
            NotNull(selector);
            NotNull(sum);

            if (values.Sum(selector) != sum)
            {
                throw new AssertionFailedException(memberName, "Sum");
            }
        }

        public static void NotNull(object value, [CallerMemberName] string memberName = "")
        {
            if (value is null)
            {
                throw new AssertionFailedException(memberName, "NotNull");
            }
        }
    }
}
