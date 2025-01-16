namespace ExamScoreApp.Core.Application.Services.Interfaces
{
    public interface IExamScoreService
    {
        Task<string> GenerateRightAnswerAsync(string question, CancellationToken cancellationToken);
        Task<string> GenerateScoreAsync(string rightAnswer, string answer, CancellationToken cancellationToken);
        Task<string> GenerateScoreFromFactsAsync(string rightFacts, string facts, CancellationToken stoppingToken);
        Task<string> GenerateFactsAsync(string statement, int numberOfFacts, CancellationToken stoppingToken);
    }
}