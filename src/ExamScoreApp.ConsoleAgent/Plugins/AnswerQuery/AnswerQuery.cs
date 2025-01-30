using System.ComponentModel;
using Microsoft.SemanticKernel;
using ExamScoreApp.Infrastructure.Interfaces;

namespace ExamScoreApp.ConsoleAgent.Plugins;

public class AnswerQuery(IContextRepository contextRepository)
{
    private readonly IContextRepository _contextRepository = contextRepository;

    [KernelFunction("get_answer")]
    [Description("Get the answer for a question")]
    async Task<string> GetAnswer(string question)
    {
        var context = await _contextRepository.GetNotesAsync(CancellationToken.None);

        return "Finance is important because it is essentia to the management of a business or organization, and it helps in running a successful business by ensuring good financial protocol, safeguards, and tools are in place.";
    } 
}