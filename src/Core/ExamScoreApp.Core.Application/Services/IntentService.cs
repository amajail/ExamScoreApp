using System.Text;
using ExamScoreApp.Core.Application.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace ExamScoreApp.Core.Application.Services
{
    public class IntentService([FromKeyedServices("ExamScoreKernel")] Kernel semanticKernel,
                               IExamScoreService ExamScoreService,
                               ILogger<IntentService> logger)
                               : IIntentService
    {
        private readonly Kernel _kernel = semanticKernel;
        private readonly KernelPlugin _intentPlugins = semanticKernel.ImportPluginFromPromptDirectory("Prompts/IntentPlugins");
        private readonly IExamScoreService _ExamScoreService = ExamScoreService;
        private List<string> _notes = [];
        private readonly ILogger<IntentService> _logger = logger;
        private ChatHistory _chatHistory = [];

        public async Task<string> RouteIntent(string input, CancellationToken cancellationToken)
        {

            _chatHistory.AddUserMessage(input);

            var intent = string.Equals(input, "exit", StringComparison.OrdinalIgnoreCase)
                ? input
                : await _kernel.InvokeAsync<string>(
                    _intentPlugins["GetIntent"],
                    new() { { "input", input } }
                );

            string response = string.Empty;

            _logger.LogInformation($"{intent} intent detected.", intent);

            switch (intent)
            {
                default:
                    var otherIntentResult = await _kernel.InvokePromptAsync(input);
                    response = otherIntentResult.ToString();
                    break;
            }

            return response;
        }

        private static string ChatHistoryToString(ChatHistory chatHistory)
        {
            // Get the new messages added to the chat history object
            StringBuilder chatHistoryStringBuilder = new();
            for (int i = 0; i < chatHistory.Count; i++)
            {
                chatHistoryStringBuilder.AppendLine(chatHistory[i].Role.ToString() + " > " + chatHistory[i].Content);
            }
            return chatHistoryStringBuilder.ToString();
        }
    }
}