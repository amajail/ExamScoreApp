
using ExamScoreApp.Core.Application.Services.Interfaces;
using Microsoft.SemanticKernel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ExamScoreApp.Core.Application.Services
{
    public class ExamScoreService([FromKeyedServices("ExamScoreKernel")] Kernel semanticKernel,
    ILogger<ExamScoreService> logger) : IExamScoreService
    {
        private readonly ILogger _logger = logger;
        private readonly Kernel _kernel = semanticKernel;
        public Task<string> GenerateRightAnswerAsync(string question, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        // Method to generate the interview report
        public async Task<string> GenerateScoreAsync(string rightAnswer,
                                                     string answer,
                                                     CancellationToken stoppingToken)
        {
            try
            {
                _logger.LogInformation($"Generating Score: Right answer: {rightAnswer} Student answer {answer}", rightAnswer, answer);

                var promptTemplate = "";

                var scoreFunction = _kernel.CreateFunctionFromPrompt(promptTemplate);

                var score = await scoreFunction.InvokeAsync(_kernel, new() {
                    {"rightAnswer", rightAnswer},
                    {"answer", answer}
                    }, stoppingToken);

                return score.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating score");
                return string.Empty;
            }
        }

        public async Task<string> GenerateScoreFromFactsAsync(string rightFacts,
                                                     string facts,
                                                     CancellationToken stoppingToken)
        {
            try
            {
                _logger.LogInformation("Generating Score from facts: Right facts: {RightFacts} Student facts {Facts}", rightFacts, facts);

                var promptTemplate =
                @"Compare this facts that are true {{$rightFacts}}

                with this ones from the user {{$facts}}

                Score the facts from 1 to 10
                Respond: Your score is (score).
                And a sentence to explaining why that score.";

                var scoreFunction = _kernel.CreateFunctionFromPrompt(promptTemplate);

                var score = await scoreFunction.InvokeAsync(_kernel, new() {
                    {"rightFacts", rightFacts},
                    {"facts", facts}
                    }, stoppingToken);

                return score.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating score");
                return string.Empty;
            }
        }

        public async Task<string> GenerateFactsAsync(string statement,
                                                     int numberOfFacts,
                                                     CancellationToken stoppingToken)
        {
            try
            {
                _logger.LogInformation("Generating facts from statement: {Statement}", statement);

                var scorePlugins = _kernel.ImportPluginFromPromptDirectory("Prompts/ScorePlugins");

                // Construct arguments
                var arguments = new KernelArguments() { ["statement"] = statement, ["numberOfFacts"] = numberOfFacts };

                // Run the Function called Joke
                var result = await _kernel.InvokeAsync(scorePlugins["GetFacts"], arguments);

                return result.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating score");
                return string.Empty;
            }
        }
    }
}
