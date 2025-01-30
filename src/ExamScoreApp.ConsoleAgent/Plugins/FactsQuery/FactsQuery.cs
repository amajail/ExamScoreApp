using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace ExamScoreApp.ConsoleAgent.Plugins;
public class FactsQuery
{
    [KernelFunction("get_facts")]
    [Description("Get facts from a statement")]
    static async Task<string> GetFacts(Kernel kernel,
                           [Description("A statement to extract the facts")] string statement)
    {
        var numberOfFacts = 3;


        CancellationToken stoppingToken = default;

        // Construct arguments
        var arguments = new KernelArguments()
        {
            ["statement"] = statement,
            ["numberOfFacts"] = numberOfFacts
        };

        // Run the Function called Get Facts
        var result = await kernel.InvokeAsync("ScorePlugins","GetFacts", arguments, stoppingToken);

        return result.ToString();
    }
}