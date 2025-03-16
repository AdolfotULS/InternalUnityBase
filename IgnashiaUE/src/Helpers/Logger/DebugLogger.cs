using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace IgnashiaUE.src.Helpers.Logger
{
    class DebugLogger
    {
        // Native
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool AllocConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool FreeConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        // Number of frames to skip in stack trace (to get calling method, not logger methods)
        private const int StackFramesToSkip = 3;

        // Logger 
        public enum LogLevel
        {
            TRACE,
            DEBUG,
            INFO,
            WARN,
            ERROR,
            FATAL,
            OFF
        }

        private static bool consoleInitialized = false;
        private static bool loggerInitialized = false;

        // Console
        private const int SW_HIDE = 0;
        private const int SW_SHOW = 5;

        // Default Settings
        public static LogLevel currentLogLevel = LogLevel.TRACE;
        public static bool useFileLogging = false;
        public static bool useConsoleLogging = true;
        public static string logFilePath = "mod_log.txt";


        public static void Initialize(LogLevel logLevel = LogLevel.TRACE, bool useConsole = true, bool useFile = false, string logFile = null)
        {
            try
            {
                currentLogLevel = logLevel;
                useConsoleLogging = useConsole;
                useFileLogging = useFile;

                if (useFile)
                {
                    if (logFile != null)
                    {
                        logFile = logFile;
                    }
                }
                else
                {
                    logFile = string.Empty;
                }

                if (useConsole)
                {
                    InitializeExternalConsole();
                }
                loggerInitialized = true;
            }
            catch
            {
                loggerInitialized = false;
            }
        }

        public static void Log(LogLevel level, string message)
        {
            if (!loggerInitialized)
            {
                Initialize();
            }

            if (level < currentLogLevel)
                return;

            string callerInfo = GetCallerMethodInfo();
            var logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} [{level}] [{callerInfo}] {message}";

            if (useConsoleLogging)
            {
                PrintToConsole(level, logMessage);
            }

            if (useFileLogging)
            {
                AppendLogToFile(logMessage);
            }
        }


        public static void Debug(string message)
        {
            Log(LogLevel.DEBUG, message);
        }

        public static void Trace(string message)
        {
            Log(LogLevel.TRACE, message);
        }

        public static void Info(string message)
        {
            Log(LogLevel.INFO, message);
        }

        public static void Warn(string message)
        {
            Log(LogLevel.WARN, message);
        }

        public static void Error(string message)
        {
            Log(LogLevel.ERROR, message);
        }

        public static void Fatal(string message)
        {
            Log(LogLevel.FATAL, message);
        }

        private static string GetCallerMethodInfo()
        {
            try
            {
                StackTrace stackTrace = new StackTrace(StackFramesToSkip, true);
                StackFrame frame = stackTrace.GetFrame(0);

                if (frame != null)
                {
                    var method = frame.GetMethod();
                    if (method != null)
                    {
                        string className = method.DeclaringType != null ? method.DeclaringType.Name : "UnknownClass";
                        string methodName = method.Name;
                        int lineNumber = frame.GetFileLineNumber();

                        if (lineNumber > 0)
                            return $"{className}.{methodName}:Line {lineNumber}";
                        else
                            return $"{className}.{methodName}";
                    }
                }
                return "Unknown";
            }
            catch
            {
                return "Unknown";
            }
        }

        private static void PrintToConsole(LogLevel level, string message)
        {
            if (!consoleInitialized)
            {
                InitializeExternalConsole();
                if (!consoleInitialized)
                {
                    return;
                }
            }

            try
            {
                switch (level)
                {
                    case LogLevel.DEBUG:
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        break;
                    case LogLevel.TRACE:
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        break;
                    case LogLevel.INFO:
                        Console.ForegroundColor = ConsoleColor.Green;
                        break;
                    case LogLevel.WARN:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        break;
                    case LogLevel.ERROR:
                        Console.ForegroundColor = ConsoleColor.Red;
                        break;
                    case LogLevel.FATAL:
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        break;
                    case LogLevel.OFF:
                        break;
                    default:
                        Console.ResetColor();
                        break;
                }

                Console.WriteLine(message);
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                AppendLogToFile($"Console writing error: {ex.Message}");
            }
        }

        private static void AppendLogToFile(string message)
        {
            try
            {
                File.AppendAllText(logFilePath, message + Environment.NewLine, Encoding.UTF8);
            }
            catch
            {
            }
        }

        private static void InitializeExternalConsole()
        {
            if (consoleInitialized)
                return;

            try
            {
                AllocConsole();
                var consoleWindow = GetConsoleWindow();

                if (consoleWindow != IntPtr.Zero)
                {
                    ShowWindow(consoleWindow, SW_SHOW);
                    Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
                    Console.OutputEncoding = Encoding.UTF8;
                    consoleInitialized = true;
                }
                consoleInitialized = true;
            }
            catch
            {
                consoleInitialized = false;
            }
        }

        public static void CloseConsole()
        {
            if (!consoleInitialized)
                return;

            try
            {
                FreeConsole();
                consoleInitialized = false;
            }
            catch
            {
            }
        }
    }
}
