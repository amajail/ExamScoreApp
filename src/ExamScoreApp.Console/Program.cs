using ExamScoreApp.Core.Application.Services;
using ExamScoreApp.Core.Application.Services.Interfaces;
using ExamScoreApp.Core.Domain.Interview;
using ExamScoreApp.Infrastructure;
using ExamScoreApp.Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
    .Build();

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // Register services
        services.AddTransient<IExamScoreService, ExamScoreService>();
        services.AddTransient<IIntentService, IntentService>();
        services.AddTransient<IQuestionRepository, QuestionRepository>();
        services.AddSingleton<IChatCompletionService>(sp =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            var apiKey = configuration["SemanticKernel:ApiKey"];
            var apiUrl = configuration["SemanticKernel:ApiUrl"];
            var chatDeploymentName = configuration["SemanticKernel:ChatDeploymentName"];
            return new AzureOpenAIChatCompletionService(chatDeploymentName!, apiUrl!, apiKey!);
        });

        services.AddKeyedTransient<Kernel>("ExamScoreKernel", (sp, key) =>
        {
            // Create a collection of plugins that the kernel will use
            KernelPluginCollection pluginCollection = new();
            return new Kernel(sp, pluginCollection);
        });

        // Add enterprise components
        services.AddLogging(services => services.AddConsole().SetMinimumLevel(LogLevel.Trace));
    })
    .Build();

// Get the service and use it
var intentService = host.Services.GetRequiredService<IIntentService>();

Console.WriteLine("I am an AI Interview Assistant. You can send notes to create a report or ask for questions.");

string? input = string.Empty;
InterviewRouterResponse result = new();

do
{
    Console.Write("User > ");
    input = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(input))
    {
        continue;
    }

    result = await intentService.RouteIntent(input, new CancellationToken());

    if (result.Questions.Count > 0)
    {
        Console.WriteLine("Questions:");
        foreach (var question in result.Questions)
        {
            Console.WriteLine("Assistant > Aqui tienes las preguntas:\n" + question);
        }
    }
    else if (!string.IsNullOrEmpty(result.Report))
    {
        Console.WriteLine("Assistant > Aqui tienes el reporte:\n" + result.Report);
    }
    else
    {
        Console.WriteLine("Assistant > Added your input as a note.");
    }
}
while (!string.Equals(input, "exit", StringComparison.OrdinalIgnoreCase));

Console.WriteLine("Goodbye!");

Console.WriteLine("This is the chat history:\n" + result.ChatHistory);
