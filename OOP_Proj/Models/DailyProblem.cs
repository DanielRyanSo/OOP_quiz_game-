namespace GameAuth.Models
{
    public class DailyProblem
    {
        public int Id { get; set; }
        public string DayKey { get; set; } = "";
        public string QuestionText { get; set; } = "";
        public string Answer { get; set; } = "";
    }
}