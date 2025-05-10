using Microsoft.AspNetCore.SignalR;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using SemanticKernelSignalRChatDemo.SignalRHubs;

namespace SemanticKernelSignalRChatDemo.Services
{
    public class AIService(IHubContext<AIHub> hubContext, IChatCompletionService chatCompletionService, Kernel kernel)
    {
        public async Task GetMessageStreamAsync(string prompt, string connectionId, CancellationToken? cancellationToken = default!)
        {
            OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
            };

            var history = HistoryService.GetChatHistory(connectionId);

            history.AddUserMessage(prompt);
            string responseContent = "";
            try
            {
                await foreach (var response in chatCompletionService.GetStreamingChatMessageContentsAsync(history, executionSettings: openAIPromptExecutionSettings, kernel: kernel))
                {
                    cancellationToken?.ThrowIfCancellationRequested();

                    await hubContext.Clients.Client(connectionId).SendAsync("ReceiveMessage", response.ToString());
                    responseContent += response.ToString();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
            history.AddAssistantMessage(responseContent);
        }
    }

    public static class HistoryService
    {
        private static readonly Dictionary<string, ChatHistory> _chatHistories = new();
        public static ChatHistory GetChatHistory(string connectionId)
        {
            ChatHistory? chatHistory = null;
            if (_chatHistories.TryGetValue(connectionId, out chatHistory))
                return chatHistory;
            else
            {
                chatHistory = new();
                _chatHistories.Add(connectionId, chatHistory);
            }
            return chatHistory;
        }
    }
}
