using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace ExamScoreApp.ConsoleAgent.Plugins;
public class ScoreQuery
{
    [KernelFunction("get_score")]
    [Description("Get the score comparing between two facts")]
    [return: Description("The score calculated comparing two facts between them")]
    static async Task<string> GetScore(Kernel kernel,
                           [Description("The facts extracted from the student answer")] string studentFacts,
                           [Description("The facts extracted from the correct answer")] string correctFacts)
    {
        CancellationToken stoppingToken = default;

        // Construct arguments
        var arguments = new KernelArguments()
        {
            ["rightFacts"] = correctFacts,
            ["facts"] = studentFacts
        };

        // Run the Function called Get Facts
        var result = await kernel.InvokeAsync("ScorePlugins", "GetScore", arguments, stoppingToken);

        return result.ToString();
    }
}