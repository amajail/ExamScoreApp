// See https://aka.ms/new-console-template for more information
using ExamScoreApp.ConsoleAgent.Plugins;
using ExamScoreApp.Infrastructure;
using ExamScoreApp.Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

Kernel kernel = CreateKernelWithChatCompletion();

OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
{
    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
};

var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

var history = new ChatHistory();

kernel.ImportPluginFromPromptDirectory("Prompts/ScorePlugins");

history.AddSystemMessage(
    @"You are a helpful teacher assistant for score an exam. 
    I will give you pairs of exam question and student answers.
    1- Get the correct answer for the question.
    2- Get the facts from the student answer and the correct answer.
    3- Get the score comparing the student facts with the correct facts.
    4- Show the score with a justification.
    Parellelize whenever you can.");

string input = @"Question: Why Finance is important? 
Student Answer: Finance is important because it is the lifeblood of a business and it is essential for the survival and growth of a business.";

AddUserMessage(input);
await GetReply();

do
{
    Console.WriteLine("What would you like to do?");
    input = Console.ReadLine()!;

    AddUserMessage(input);
}
while (!string.IsNullOrWhiteSpace(input));

async Task GetReply()
{
    var reply = chatCompletionService.GetStreamingChatMessageContentsAsync(
        history,
        executionSettings: openAIPromptExecutionSettings,
        kernel: kernel
    );

    string fullMessage = string.Empty;
    await foreach (var content in reply)
    {
        Console.Write(content.Content);
        fullMessage += content.Content;
    }

    Console.WriteLine("Assistant: " + fullMessage.ToString());
    history.AddAssistantMessage(fullMessage.ToString());
}

void AddUserMessage(string msg)
{
    Console.WriteLine("User: " + msg);
    history.AddUserMessage(msg);
}

static Kernel CreateKernelWithChatCompletion()
{
    var configuration = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
           .AddJsonFile("appsettings.development.json", optional: true, reloadOnChange: true)
           .Build();

    var apiKey = configuration["SemanticKernel:ApiKey"];
    var apiUrl = configuration["SemanticKernel:ApiUrl"];
    var chatDeploymentName = configuration["SemanticKernel:ChatDeploymentName"];

    var serviceProvider = new ServiceCollection()
        .AddScoped<IContextRepository, ContextRepository>()
        .AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Trace))
        .BuildServiceProvider();

    var builder = Kernel.CreateBuilder();
    builder.Services.AddAzureOpenAIChatCompletion(
        chatDeploymentName!,
        apiUrl!,
        apiKey!,
        "gpt-4o");


    builder.Services.AddLogging(services => services.AddConsole().SetMinimumLevel(LogLevel.Trace));
    var kernel = builder.Build();
    kernel.Plugins.AddFromType<AnswerQuery>("AnswerQuery", serviceProvider);
    kernel.Plugins.AddFromType<FactsQuery>();
    kernel.Plugins.AddFromType<ScoreQuery>();
    return kernel;
}