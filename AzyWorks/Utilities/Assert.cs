using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace AzyWorks.Utilities
{
    public static class Assert
    {
        private const string TrueBoolUnmetMessage = "Assertion failed: Expected a true boolean.";
        private const string FalseBoolUnmetMessage = "Assertion failed: Expected a false boolean.";
        private const string EqualsUnmetMessage = "Assertion failed: The first value is not equal to the second one.";
        private const string EvenUnmetMessage = "Assertion failed: All numbers in the list were not even.";
        private const string ObjectUnmetMessage = "Assertion failed: All objects in the list were not equal.";
        private const string NotNullUnmetMessage = "Assertion failed: The specified object is null.";
        private const string TypeUnmetMessage = "Assertion failed: Object Type mismatch.";
        private const string ListNotNullMessage = "Assertion failed: Some object in the list was null.";
        private const string EitherNullMessage = "Assertion failed: Both provided objects were null.";
        private const string FileExistsMessage = "Assertion failed: Specified file does not exist.";
        private const string DirectoryExistsMessage = "Assertion failed: Specified directory does not exist.";

        public static void EitherNull(object obj1, object obj2, object message = null)
        {
            if (obj1 != null || obj2 != null)
                return;

            if (obj1 == null && obj2 == null)
            {
                ThrowHelper.LogAndThrow(message != null ? message.ToString() : EitherNullMessage);
            }
        }

        public static void FileExists(string path, object message = null)
        {
            if (!File.Exists(path))
                ThrowHelper.LogAndThrow(message != null ? message.ToString() : FileExistsMessage);
        }

        public static void DirectoryExists(string path, object message = null)
        {
            if (!Directory.Exists(path))
                ThrowHelper.LogAndThrow(message != null ? message.ToString() : DirectoryExistsMessage);
        }

        public static void NotNull(object obj, object message = null)
        {
            if (obj is null)
                return;

            ThrowHelper.LogAndThrow(message != null ? message.ToString() : NotNullUnmetMessage);
        }

        public static void AllNotNull(IEnumerable<object> list, object message = null)
        {
            if (list.Any(x => x is null))
            {
                ThrowHelper.LogAndThrow(message != null ? message.ToString() : ListNotNullMessage);
            }
        }

        public static void IsType<T>(object obj, object message = null)
        {
            if (obj is T)
                return;

            ThrowHelper.LogAndThrow(message != null ? message.ToString() : TypeUnmetMessage);
        }

        public static void True(bool value, object messageIfFalse = null)
        {
            if (value)
                return;

            ThrowHelper.LogAndThrow(messageIfFalse != null ? messageIfFalse.ToString() : TrueBoolUnmetMessage);
        }

        public static void False(bool value, object messageIfFalse = null)
        {
            if (!value)
                return;

            ThrowHelper.LogAndThrow(messageIfFalse != null ? messageIfFalse.ToString() : FalseBoolUnmetMessage);
        }

        public static void Equals(float num1, float num2, object message = null)
        {
            if (num1 == num2)
                return;

            ThrowHelper.LogAndThrow(message != null ? message.ToString() : EqualsUnmetMessage);
        }

        public static void Equals(object obj1, object obj2, object message = null)
        {
            if (obj1 == obj2)
                return;

            ThrowHelper.LogAndThrow(message != null ? message.ToString() : EqualsUnmetMessage);
        }

        public static void Even(IEnumerable<float> numbers, object message = null)
        {
            if (numbers.Count() <= 0)
                return;

            if (numbers.All(x => x == numbers.First()))
                return;

            ThrowHelper.LogAndThrow(message != null ? message.ToString() : EvenUnmetMessage);
        }

        public static void Equal(IEnumerable<object> list, object message = null)
        {
            if (list.Count() <= 0)
                return;

            if (list.All(x => x == list.First()))
                return;

            ThrowHelper.LogAndThrow(message != null ? message.ToString() : ObjectUnmetMessage);
        }
    }
}
