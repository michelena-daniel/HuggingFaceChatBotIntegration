using HuggingFaceChatApp.src.Application.Interfaces;

namespace HuggingFaceChatApp.src.ConsoleApp.Services
{
    public class ConsoleChatService
    {
        private readonly IChatService _chatService;

        public ConsoleChatService(IChatService chatService)
        {
            _chatService = chatService;
        }

        public async Task RunAsync()
        {
            Console.WriteLine("Waking up Plant Care Bot");
            while (true)
            {
                Console.Write("Enter your prompt: ");
                string prompt = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(prompt))
                    continue;
                string response = await _chatService.GetChatResponseAsync(prompt);

                Console.WriteLine(response);
            }
        }
    }
}
