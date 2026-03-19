namespace AuthApi.Dtos
{
    public class EventMessage
    {
        public  string  Service { get; set; }
        public  string  Action { get; set; }
        public  DateTime Timestamp { get; set; }
    }
}
