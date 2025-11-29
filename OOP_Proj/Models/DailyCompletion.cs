namespace GameAuth.Models
{
    public class DailyCompletion
    {
        public int Id { get; set; }
        public string Username { get; set; } = "";
        public string DayKey { get; set; } = "";
        public bool Completed { get; set; }
        public DateTime CompletionTime { get; set; }
    }
}