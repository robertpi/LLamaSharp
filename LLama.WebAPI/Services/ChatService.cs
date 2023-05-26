using System.Text.RegularExpressions;
using LLama.WebAPI.Models;
using LlamaSharp.WebApp.Hubs;
using LlamaSharp.WebApp.Models;
using Microsoft.AspNetCore.SignalR;

namespace LLama.WebAPI.Services;

public class ChatService
{
    private readonly ChatSession<LLamaModel> _session;
    private readonly IHubContext<ChatHub> _hubContext;

    public ChatService(IHubContext<ChatHub> hubContext)
    {
        _hubContext = hubContext;
        LLamaModel model = new(new LLamaParams(model: @"../models/ggml-alpaca-7b-q4.bin", n_ctx: 512, interactive: true, repeat_penalty: 1.0f, verbose_prompt: false));
        _session = new ChatSession<LLamaModel>(model)
            .WithPromptFile(@"../Llama.Examples/Assets/alpaca.txt");
    }

    public async Task<string> Send(SendMessageInput input)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(input.Text);

        Console.ForegroundColor = ConsoleColor.White;
        var outputs = _session.Chat(input.Text);
        var result = "";
        foreach (var output in outputs)
        {
            Console.Write(output);

            var resultMsg = new Message()
            {
                Id = Guid.NewGuid(),
                Content = Regex.Replace(output, @"<.*?>", string.Empty),
                FromUserName = "robot",
                Avatar = "robot-icon.jpg",
                Timestamp = DateTime.Now
            };

            // Broadcast the robots message
            await _hubContext.Clients.All.SendAsync("newMessage", resultMsg);

            result += output;
        }

        return result;
    }
}
