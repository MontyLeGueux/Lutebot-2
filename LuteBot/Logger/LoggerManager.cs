using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuteBot.Logger
{
    public class LoggerManager
    {
        private bool isOn;

        public enum LoggerLevel
        {
            Essential = 0,
            Basic = 1,
            Medium = 2,
            Complete = 3
        }

        private LoggerLevel currentLoggerLevel;

        private List<LoggerEvent> loggedEvents;

        public LoggerLevel CurrentLoggerLevel { get => currentLoggerLevel; set => currentLoggerLevel = value; }
        public bool IsOn { get => isOn; set => isOn = value; }

        public event EventHandler<LoggerEvent> InboundMessage;

        public LoggerManager()
        {
            loggedEvents = new List<LoggerEvent>();
            loggedEvents.Add(new LoggerEvent() {LoggedEventType = LoggerEvent.EventType.INFO, Sender = "LoggerManager", Message = "Logger Initialised", TimeStamp = DateTime.Now });
            currentLoggerLevel = LoggerLevel.Complete;
            GlobalLogger.Logger = this;
            isOn = true;
        }

        public void Log(LoggerEvent loggedEvent)
        {
            loggedEvents.Add(loggedEvent);
            EventHandler<LoggerEvent> handler = InboundMessage;
            handler?.Invoke(this, loggedEvent);
        }

        public string GetLoggedEvents()
        {
            string result = "";
            foreach (LoggerEvent loggerEvent in loggedEvents)
            {
                result = result + loggerEvent.ToString();
            }
            return result;
        }
    }
}
