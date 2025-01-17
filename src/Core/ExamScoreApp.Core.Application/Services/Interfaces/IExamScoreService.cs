namespace ExamScoreApp.Core.Application.Services.Interfaces
{
    public interface IExamScoreService
    {
        Task<string> GenerateScoreAsync(string question, string answer, CancellationToken cancellationToken);
        Task<string> GenerateScoreFromFactsAsync(string rightFacts, string facts, CancellationToken stoppingToken);
        Task<string> GenerateFactsAsync(string statement, int numberOfFacts, CancellationToken stoppingToken);
        Task<string> GenerateRightAnswerAsync(string context, string question, CancellationToken stoppingToken);
    }
}