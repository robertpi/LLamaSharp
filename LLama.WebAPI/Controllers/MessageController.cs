using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using LLama.WebAPI.Models;
using LLama.WebAPI.Services;
using LlamaSharp.WebApp.Hubs;
using LlamaSharp.WebApp.Models;
using LlamaSharp.WebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace LlamaSharp.WebApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MessagesController : ControllerBase
{
    private readonly IHubContext<ChatHub> _hubContext;
    private readonly ChatService _chatService;
    private readonly ConcurrentDictionary<Guid, Message> _messages = new ();

    public MessagesController(IHubContext<ChatHub> hubContext, ChatService chatService)
    {
        _hubContext = hubContext;
        _chatService = chatService;
    }

    [HttpGet("{id}")]
    public ActionResult<Message> Get(Guid id)
    {
        if (_messages.TryGetValue(id, out var msg))
        {
            return Ok(msg);
        }

        return NotFound();
    }


    [HttpPost]
    public async Task<ActionResult<Message>> Create(MessageViewModel viewModel)
    {
        var msg = new Message()
        {
            Id = Guid.NewGuid(),
            Content = Regex.Replace(viewModel.Content, @"<.*?>", string.Empty),
            FromUserName = "human",
            Avatar = "user-icon-human.png",
            Timestamp = DateTime.Now
        };

        _messages.AddOrUpdate(msg.Id, msg, (_, _) => msg);

        // Broadcast the human's message
        await _hubContext.Clients.All.SendAsync("newMessage", msg);

        _chatService.Send(new SendMessageInput { Text = msg.Content } );

        return CreatedAtAction(nameof(Get), new {id = msg.Id}, msg);
    }

    [HttpPost("Control")]
    public ActionResult<Message> Control(MessageViewModel viewModel)
    {
        // if (_llamaContextApi.Stop())
        // {
        //     var msg = new Message()
        //     {
        //         Id = Guid.NewGuid(),
        //         Content = ".",
        //         FromUserName = "robot",
        //         Avatar = "robot-icon.jpg",
        //         Timestamp = DateTime.Now
        //     };
        //     await _hubContext.Clients.All.SendAsync("newMessage", msg);
        // }

        return Ok();
    }
}
