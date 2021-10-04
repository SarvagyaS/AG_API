using AG.Hubs;
using AG.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AG.Controllers
{
    [Route("api/LiveAuction/[action]")]
    [ApiController]
    public class LiveAuctionController : ControllerBase
    {
        private readonly IHubContext<LiveAuctionHub> _hubContext;

        public LiveAuctionController(IHubContext<LiveAuctionHub> hubContext)
        {
            _hubContext = hubContext;
        }

        [HttpPost]
        public IActionResult SendGlobal([FromBody] LiveAuctionDetails liveAuctionDetails)
        {
            _hubContext.Clients.All.SendAsync("ReceiveOne", liveAuctionDetails.userId, liveAuctionDetails.price);
            return Ok();
        }

        [HttpPost]
        public IActionResult SendToGroup([FromBody] LiveAuctionDetails liveAuctionDetails)
        {
            _hubContext.Groups.AddToGroupAsync(liveAuctionDetails.productId, liveAuctionDetails.userId);
            _hubContext.Clients.Group(liveAuctionDetails.productId).SendAsync("ReceiveOne", liveAuctionDetails.userId, liveAuctionDetails.price);
            return Ok();
        }
    }
}
