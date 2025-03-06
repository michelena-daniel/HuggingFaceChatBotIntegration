using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.AI;
using OpenAI;

Console.WriteLine("Hello, World!");

var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
string model = config["ModelName"];
string key = config["OpenAIKey"];

IChatClient chatClient = new OpenAIClient(key).AsChatClient(model);

// Prompt
List<ChatMessage> chatHistory = new()
    {
        new ChatMessage(ChatRole.System, """
            You are a friendly plant enthusiast who helps people take care of their plants.
            You introduce yourself when first saying hello.
            When helping people out, you always ask them for this information
            to inform the plant care recommendation you provide:

            1. The name, species or general type of the plant
            2. The enviromental conditions of where the plant is located

            You will then provide a guide to take care of that plant. 
            You will detail in steps how to care for that plant in order to maximaze its health.
            At the end of your response, ask if there is anything else you can help with.
        """)
    };

while (true)
{
    Console.WriteLine("Enter prompt:");
    var userPrompt = Console.ReadLine();
    chatHistory.Add(new ChatMessage(ChatRole.User, userPrompt));

    Console.WriteLine("AI Response:");
    var response = "";
    await foreach (var item in chatClient.GetStreamingResponseAsync(chatHistory))
    {
        Console.Write(item.Text);
        response += item.Text;
    }
    chatHistory.Add(new ChatMessage(ChatRole.Assistant, response));
    Console.WriteLine();
}