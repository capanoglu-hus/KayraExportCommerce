using System;
using System.Collections.Generic;
using System.Text;

namespace ProductionService.Persistence
{
    public class LogDataMessage
    {
        public enum LogLevel
        {
            Information = 1,
            Warning = 2,
            Error = 3,
            Critical = 4,
            Debug = 5
        }
        public class LogMessage
        {
            public string ServiceName { get; set; } 
            public LogLevel Level { get; set; }    
            public string Message { get; set; }     
            public string? Exception { get; set; }  
            public DateTime Timestamp { get; set; } = DateTime.UtcNow;
            public object? Payload { get; set; }  
        }

    }
}
