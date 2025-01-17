
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
        private KernelPlugin _scorePlugin = semanticKernel.ImportPluginFromPromptDirectory("Prompts/ScorePlugins");

        // Method to generate the interview report
        public async Task<string> GenerateScoreAsync(string question,
                                                     string answer,
                                                     CancellationToken stoppingToken)
        {
            try
            {
                var numberOfFacts = 5;
                var context = "Finance is essential to the management of a business or organization, as it ensures good financial protocol, safeguards, and tools, making it easier to run a successful business.";

                _logger.LogInformation($"Generating Score: Question: {question} Student answer {answer}", question, answer);

                // Construct arguments
                var arguments = new KernelArguments()
                {
                    ["context"] = context,
                    ["question"] = question,
                    ["answer"] = answer,
                    ["numberOfFacts"] = numberOfFacts
                };

                // Run the Function
                var score = await _kernel.InvokeAsync(_scorePlugin["GetScore"], arguments, stoppingToken);

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

                // Construct arguments
                var arguments = new KernelArguments()
                {
                    ["rightFacts"] = rightFacts,
                    ["facts"] = facts
                };

                // Run the Function called Get Facts
                var result = await _kernel.InvokeAsync(_scorePlugin["GetFacts"], arguments);

                return result.ToString();
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

                // Construct arguments
                var arguments = new KernelArguments()
                {
                    ["statement"] = statement,
                    ["numberOfFacts"] = numberOfFacts
                };

                // Run the Function called Get Facts
                var result = await _kernel.InvokeAsync(_scorePlugin["GetFacts"], arguments, stoppingToken);

                return result.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating score");
                return string.Empty;
            }
        }

        public async Task<string> GenerateRightAnswerAsync(string context,
                                                           string question,
                                                           CancellationToken stoppingToken)
        {
            try
            {
                _logger.LogInformation("Generating right answer from question: {Question}", question);

                // Construct arguments
                var arguments = new KernelArguments()
                {
                    ["context"] = context,
                    ["question"] = question
                };

                // Run the Function called Get Facts
                var result = await _kernel.InvokeAsync(_scorePlugin["GetRightAnswer"], arguments, stoppingToken);

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
