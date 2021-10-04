using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AG.Hubs
{
    public class LiveAuctionHub : Hub
    {
        //public override Task OnConnectedAsync()
        //{
        //    Groups.AddToGroupAsync(Context.ConnectionId, Context.User.Identity.Name);
        //    return base.OnConnectedAsync();
        //}

        public Task SendGlobal(string user, string price)
        {
            return Clients.All.SendAsync("ReceiveOne", user, price);
        }

        public Task SendToGroup(string sender, string receiver, string message)
        {
            return Clients.Group(receiver).SendAsync("ReceiveMessage", sender, message);
        }
    }
}
