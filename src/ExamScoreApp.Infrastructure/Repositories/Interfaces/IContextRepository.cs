namespace ExamScoreApp.Infrastructure.Interfaces;
public interface IContextRepository
{
    Task<string> GetNotesAsync(CancellationToken stoppingToken);
}