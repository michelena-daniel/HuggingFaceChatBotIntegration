﻿namespace HuggingFaceChatApp.src.Application.Interfaces
{
    public interface IChatService
    {
        Task<string> GetChatResponseAsync(string userMessage);
    }
}
