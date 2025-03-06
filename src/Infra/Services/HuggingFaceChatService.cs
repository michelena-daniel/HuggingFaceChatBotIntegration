using HuggingFaceChatApp.src.Application.Interfaces;
using System.Text;
using System.Text.Json;

namespace HuggingFaceChatApp.src.Infra.Services
{
    public class HuggingFaceChatService : IChatService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _model;
        private List<string> _chatHistory = new()
        {
            "You are a friendly plant enthusiast who helps people take care of their plants.\r\nYou introduce yourself when first saying hello.\r\nWhen helping people out, you first ask them for this information\r\nto inform the plant care recommendation you provide:\r\n\r\n1. The name, species or general type of the plant\r\n2. The enviromental conditions of where the plant is located\r\n\r\nYou will then provide a guide to take care of that plant only if a plant species or indication is given to you. \r\nIf so, you will detail in steps how to care for that plant in order to maximaze its health.\r\nAt the end of your response, ask if there is anything else you can help with."
        };

        public HuggingFaceChatService(HttpClient httpClient, string apiKey, string model)
        {
            _httpClient = httpClient;
            _apiKey = apiKey;
            _model = model;
        }

        public async Task<string> GetChatResponseAsync(string userMessage)
        {
            try
            {
                string formattedHistory = string.Join("\n", _chatHistory);
                string fullPrompt = $"{formattedHistory}\n{userMessage}";

                var requestBody = new { inputs = fullPrompt };
                var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

                string apiUrl = $"https://api-inference.huggingface.co/models/{_model}";

                var response = await _httpClient.PostAsync(apiUrl, content);
                if (!response.IsSuccessStatusCode)
                {
                    string errorResponse = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error: {response.StatusCode}, {errorResponse}");
                    return $"Error: {response.StatusCode}";
                }
                string responseString = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(responseString);
                var root = doc.RootElement;
                string fullGeneratedText = root[0].GetProperty("generated_text").GetString();

                // Extract original prompt and return only the bot's response
                string assistantResponse = ExtractAssistantResponse(formattedHistory, fullGeneratedText);

                // We store the prompt's history
                _chatHistory.Add($"\n{userMessage}\n");
                _chatHistory.Add($"\n{assistantResponse}\n");

                return assistantResponse;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        private static string ExtractAssistantResponse(string prompt, string fullText)
        {
            if (fullText.StartsWith(prompt))
            {
                return fullText.Substring(prompt.Length).Trim();
            }
            return fullText.Trim();
        }
    }
}
