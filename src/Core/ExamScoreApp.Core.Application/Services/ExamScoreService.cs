
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

        public async Task<string> GenerateScoreAsync(string question,
                                                     string answer,
                                                     CancellationToken stoppingToken)
        {
            try
            {
                _logger.LogInformation($"Generating Score: Question: {question} Student answer {answer}", question, answer);

                var response = await GenerateFactsToScoreAsync(question, answer, stoppingToken);

                // Construct arguments
                var arguments = new KernelArguments()
                {
                    ["rightFacts"] = response.RightFacts,
                    ["userFacts"] = response.UserFacts,
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

        private async Task<FactsResponse> GenerateFactsToScoreAsync(string question, string answer, CancellationToken stoppingToken)
        {
            var numberOfFacts = 5;
            var context = @" Finance is essential to the management of a business or organization. Without good financial protocol,
            safeguards, and tools, running a successful business is more difficult. In 1978, Bacon Signs was a family
            owned, regional Midwestern sign company engaged in the manufacture, sale, installation, and maintenance of
            commercial signage. The company was about to transition from the second to third generation of family
            ownership. Bacon Signs, established in 1901, had weathered the Great Depression, World War II, the Vietnam
            War, and the oil embargo and was working its way through historically high rates of inflation and interest
            rates. The family business had successfully struggled through the ebb and flow of the regional and national
            economy by providing quality products and service to its regional clients.
            In the early 1980s, the company’s fortunes changed permanently for the better. The owner recognized that
            the custom signs built by his firm were superior in quality to the signs it installed for national franchises. The
            owner worked with the company’s banker and vice president of finance and operations to develop a
            production, sales, and financing plan that could be offered to the larger national sign companies. The larger
            companies agreed to subcontract manufacturing of midsize orders to Bacon Signs. The firm then made a
            commitment to build and deliver these signs on time and under budget. As Bacon Signs’ reputation for quality
            grew, so did demand for its products. The original financing plan anticipated this potential growth and was
            designed to meet anticipated capital requirements so that the firm could expand how and when it needed to.
            Bacon Signs’ ability to manufacture and deliver a high-quality product at a good price was the true value of the
            firm. However, without the planning and ability to raise capital facilitated by the financing plan, the firm would
            not have been able to act on its strengths at the critical moment. Financing was the key to expansion and
            financial stability for the firm.1
            In this book, we demonstrate that business finance is about developing and understanding the tools that help
            people make consistently good and repeatable decisions";

            var rightAnswer = await GenerateRightAnswerAsync(context, question, stoppingToken);

            var rightFactsTask = GenerateFactsAsync(rightAnswer, numberOfFacts, stoppingToken);
            var userFactsTask = GenerateFactsAsync(answer, numberOfFacts, stoppingToken);

            await Task.WhenAll(rightFactsTask, userFactsTask);

            return new FactsResponse
            {
                RightFacts = await rightFactsTask,
                UserFacts = await userFactsTask
            };
        }
    }


    internal class FactsResponse
    {
        public string RightFacts { get; set; }
        public string UserFacts { get; set; }
    }
}
