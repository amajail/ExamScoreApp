using ExamScoreApp.Infrastructure.Interfaces;

namespace ExamScoreApp.Infrastructure;

public class ContextRepository : IContextRepository
{
    public async Task<string> GetNotesAsync(CancellationToken stoppingToken)
    {
        return await File.ReadAllTextAsync("Files/context/context.txt", stoppingToken);
    }
}

