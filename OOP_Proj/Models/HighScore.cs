namespace GameAuth.Models
{
    public class HighScore
    {
        public int Id { get; set; }
        public string Username { get; set; } = "";
        public string Operation { get; set; } = "";
        public string Difficulty { get; set; } = "";
        public int Score { get; set; }
        public bool FastMode { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
    }
}