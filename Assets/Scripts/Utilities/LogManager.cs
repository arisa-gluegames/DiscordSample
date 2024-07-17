using System;
using Debug = UnityEngine.Debug;

namespace GlueGames.Utilities
{
    public enum LogLevel
    {
        None,
        Error,
        Info,
        All,
    }

    public static class LogManager
    {
        public static LogLevel CurrentLogLevel { get; set; } = LogLevel.All;

        public static void Log(string message, LogLevel level = LogLevel.All, bool isWarning = false)
        {
            if (ShouldLog(level))
            {
                switch (level)
                {
                    case LogLevel.Info:
                    case LogLevel.All:
                        if (isWarning)
                            Debug.LogWarning(message);
                        else
                            Debug.Log(message);
                        break;
                    case LogLevel.Error:
                        Debug.LogError(message);
                        break;
                    default:
                        break;
                }
            }
        }

        public static void LogFormat(string format, LogLevel level = LogLevel.All, params object[] args)
        {
            if (ShouldLog(level))
            {
                switch (level)
                {
                    case LogLevel.Info:
                    case LogLevel.All:
                        Debug.LogFormat(format, args);
                        break;
                    case LogLevel.Error:
                        Debug.LogErrorFormat(format, args);
                        break;
                    default:
                        break;
                }
            }
        }

        private static bool ShouldLog(LogLevel level)
        {
            switch (CurrentLogLevel)
            {
                case LogLevel.None:
                    return false;
                case LogLevel.Error:
                    return level == LogLevel.Error;
                case LogLevel.Info:
                    return level == LogLevel.Info || level == LogLevel.Error;
                case LogLevel.All:
                    return true;
                default:
                    return false;
            }
        }

        public static void LogError(string message)
        {
            Log(message, LogLevel.Error);
        }

        public static void LogError(Exception exception)
        {
            Debug.LogError(exception);
        }

        public static void LogInfo(string message)
        {
            Log(message, LogLevel.Info);
        }

        public static void LogWarning(string message)
        {
            Log(message, LogLevel.All, true);
        }

        public static void LogWarningInfo(string message)
        {
            Log(message, LogLevel.Info, true);
        }

        public static void Log(string message)
        {
            Log(message, LogLevel.All);
        }

        public static void LogFormat(string format, params object[] args)
        {
            LogFormat(format, LogLevel.All, args);
        }

        public static void LogFormatInfo(string format, params object[] args)
        {
            LogFormat(format, LogLevel.Info, args);
        }

        public static void LogInfoFormat(string format, params object[] args)
        {
            LogFormat(format, LogLevel.Info, args);
        }

        public static void LogErrorFormat(string format, params object[] args)
        {
            LogFormat(format, LogLevel.Error, args);
        }
    }
}

