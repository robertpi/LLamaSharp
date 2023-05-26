using System;
using System.Threading.Tasks;
using LlamaSharp.WebApp.ViewModels;
using Microsoft.AspNetCore.SignalR;

namespace LlamaSharp.WebApp.Hubs;

public class ChatHub : Hub
{
    public override Task OnConnectedAsync()
    {
        try
        {
            var userViewModel = new UserViewModel()
            {
                UserName = "human",
                FullName = "human",
                Avatar = "user-icon-human.png",
                CurrentRoom = "livingroom",
                Device = "my-device"
            };

            Clients.Caller.SendAsync("getProfileInfo", userViewModel);
        }
        catch (Exception ex)
        {
            Clients.Caller.SendAsync("onError", "OnConnected:" + ex.Message);
        }
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception exception)
    {
        try
        {
        }
        catch (Exception ex)
        {
            Clients.Caller.SendAsync("onError", "OnDisconnected: " + ex.Message);
        }

        return base.OnDisconnectedAsync(exception);
    }

}