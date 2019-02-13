using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuteBot.Logger
{
    public class LoggerEvent
    {
        public enum EventType
        {
            WARN,
            ERROR,
            INFO
        }

        private EventType loggedEventType;
        private DateTime timeStamp;
        private string sender;
        private string message;
        private LoggerManager.LoggerLevel logLevel;

        public EventType LoggedEventType { get => loggedEventType; set => loggedEventType = value; }
        public DateTime TimeStamp { get => timeStamp; set => timeStamp = value; }
        public string Sender { get => sender; set => sender = value; }
        public string Message { get => message; set => message = value; }
        public LoggerManager.LoggerLevel LogLevel { get => logLevel; set => logLevel = value; }

        public override string ToString()
        {
            string result = FormatTimeStamp()+" "+EventTypeToString(LoggedEventType)+" - ("+sender+"):"+message+"\n";

            return result;
        }

        private string FormatTimeStamp()
        {
            string result = "[" + timeStamp.Year + "/" + timeStamp.Month + "/" + timeStamp.Day + "-" + timeStamp.Hour + ":" + timeStamp.Minute+":"+timeStamp.Second+"-MS:"+timeStamp.Millisecond+"]";

            return result;
        }

        private string EventTypeToString(EventType type)
        {
            string result = "NONE";
            if (type == EventType.ERROR)
            {
                result = "ERROR";
            }
            else if (type == EventType.INFO)
            {
                result = "INFO";
            }
            else if (type == EventType.WARN)
            {
                result = "WARN";
            }
            return result;
        }
    }
}
