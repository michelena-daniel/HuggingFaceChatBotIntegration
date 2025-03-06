using Microsoft.Extensions.DependencyInjection;
using HuggingFaceChatApp.src.Infra.Services;
using HuggingFaceChatApp.src.ConsoleApp.Services;
using HuggingFaceChatApp.src.Application.Interfaces;

class Program
{
    static async Task Main()
    {
        var serviceProvider = new ServiceCollection()
            .AddSingleton<HttpClient>()
            .AddSingleton<IChatService>(sp => new HuggingFaceChatService(sp.GetRequiredService<HttpClient>(), "api-key", "mistralai/Mistral-7B-Instruct-v0.3"))
            .AddSingleton<ConsoleChatService>()
            .BuildServiceProvider();
        var chatService = serviceProvider.GetRequiredService<ConsoleChatService>();
        await chatService.RunAsync();
    }
}