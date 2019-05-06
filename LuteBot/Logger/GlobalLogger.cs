using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LuteBot.Logger
{
    class GlobalLogger
    {
        private static LoggerManager logger;

        public static LoggerManager Logger { get => logger; set => logger = value; }

        public static void Log(string sender, LoggerManager.LoggerLevel severity, string message)
        {
            LoggerEvent result = BuildLoggerEvent(LoggerEvent.EventType.INFO, sender, severity, message);
            if (result != null)
            {
                ThreadPool.QueueUserWorkItem(ThreadPoolLog, result);
            }
        }

        public static void Warn(string sender, LoggerManager.LoggerLevel severity, string message)
        {
            LoggerEvent result = BuildLoggerEvent(LoggerEvent.EventType.WARN, sender, severity, message);
            if (result != null)
            {
                ThreadPool.QueueUserWorkItem(ThreadPoolLog, result);
            }
        }
        public static void Error(string sender, LoggerManager.LoggerLevel severity, string message)
        {
            LoggerEvent result = BuildLoggerEvent(LoggerEvent.EventType.ERROR, sender, severity, message);
            if (result != null)
            {
                ThreadPool.QueueUserWorkItem(ThreadPoolLog, result);
            }
        }

        private static LoggerEvent BuildLoggerEvent(LoggerEvent.EventType eventType, string sender, LoggerManager.LoggerLevel severity, string message)
        {
            LoggerEvent result = null;
            if (logger != null && logger.IsOn && severity <= logger.CurrentLoggerLevel)
            {
                logger.Log(new LoggerEvent() {LoggedEventType = eventType, Message = message, Sender = sender, TimeStamp = DateTime.Now, LogLevel = severity });
            }
            return result;
        }

        private static void ThreadPoolLog(object logInfo)
        {
            logger.Log((LoggerEvent) logInfo);
        }
    }
}
