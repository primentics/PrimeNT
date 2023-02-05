using System;

namespace AzyWorks.Logging
{
    public static class LogStream
    {
        public static event Action<string, string, string> OnMessageLogged;

        public static void LogToConsole()
        {
            OnMessageLogged += (x, y, z) =>
            {
                switch (x)
                {
                    case "Info":
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"[{DateTime.UtcNow.ToString("t")}] [{x}] [{y}]: {z}");
                            Console.ResetColor();

                            break;
                        }

                    case "Debug":
                        {
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.WriteLine($"[{DateTime.UtcNow.ToString("t")}] [{x}] [{y}]: {z}");
                            Console.ResetColor();

                            break;
                        }

                    case "Error":
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"[{DateTime.UtcNow.ToString("t")}] [{x}] [{y}]: {z}");
                            Console.ResetColor();

                            break;
                        }

                    case "Warn":
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine($"[{DateTime.UtcNow.ToString("t")}] [{x}] [{y}]: {z}");
                            Console.ResetColor();

                            break;
                        }
                }
            };
        }

        internal static void Log(string tag, string source, string message)
            => OnMessageLogged?.Invoke(tag, source, message);
    }
}