namespace LogWebApi.Dto
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
            public string ServiceName { get; set; } // Hangi servisten geldi? (Auth, Production)
            public LogLevel Level { get; set; }    // INFO, WARNING, ERROR, CRITICAL
            public string Message { get; set; }     // Log mesajı
            public string? Exception { get; set; }  // Varsa hata detayları (StackTrace)
            public DateTime Timestamp { get; set; } = DateTime.UtcNow;
            public object? Payload { get; set; }    // İsteğe bağlı: Gönderilen data (JSON)
        }

        
    }
}
