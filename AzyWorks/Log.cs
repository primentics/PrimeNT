using AzyWorks.Logging;

using System.Collections.Generic;

namespace AzyWorks
{
    public static class Log
    {
        public static IReadOnlyList<string> LibrarySources { get; } = new List<string>()
        {
            "ExceptionManager",
            "ReflectionUtils",
            "Code Compiler",
            "BinaryTools",
            "UnityModule"
        };

        public static HashSet<string> BlacklistedSources { get; } = new HashSet<string>();
        public static HashSet<string> BlacklistedLevels { get; } = new HashSet<string>() { "Debug" };

        public static void SendInfo(object source, object message)
        {
            var sourceStr = source.ToString();
            var messageStr = message.ToString();

            if (BlacklistedSources.Contains(sourceStr))
                return;

            if (BlacklistedLevels.Contains("Info"))
                return;

            LogStream.Log("Info", sourceStr, messageStr);
        }

        public static void SendError(object source, object message)
        {
            var sourceStr = source.ToString();
            var messageStr = message.ToString();

            if (BlacklistedSources.Contains(sourceStr))
                return;

            if (BlacklistedLevels.Contains("Error"))
                return;

            LogStream.Log("Error", sourceStr, messageStr);
        }

        public static void SendDebug(object source, object message)
        {
            var sourceStr = source.ToString();
            var messageStr = message.ToString();

            if (BlacklistedSources.Contains(sourceStr))
                return;

            if (BlacklistedLevels.Contains("Debug"))
                return;

            LogStream.Log("Debug", sourceStr, messageStr);
        }

        public static void SendWarn(object source, object message)
        {
            var sourceStr = source.ToString();
            var messageStr = message.ToString();

            if (BlacklistedSources.Contains(sourceStr))
                return;

            if (BlacklistedLevels.Contains("Warn"))
                return;

            LogStream.Log("Warn", sourceStr, messageStr);
        }
    }
}