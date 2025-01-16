namespace ExamScoreApp.Api.Models
{
    public class ExamScoreResponse
    {
        public string Report { get; set; }
        public string ChatHistory { get; internal set; }
    }
}