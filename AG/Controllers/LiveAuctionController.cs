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
    [Route("api/LiveAuction")]
    [ApiController]
    public class LiveAuctionController : ControllerBase
    {
        private readonly IHubContext<LiveAuctionHub> _hubContext;

        public LiveAuctionController(IHubContext<LiveAuctionHub> hubContext)
        {
            _hubContext = hubContext;
        }

        [Route("send")]                                           //path looks like this: https://localhost:44379/api/chat/send
        [HttpPost]
        public IActionResult SendRequest([FromBody] LiveAuctionDetails liveAuctionDetails)
        {
            _hubContext.Clients.All.SendAsync(liveAuctionDetails.productId + "", liveAuctionDetails.userId, liveAuctionDetails.price);
            return Ok();
        }
    }
}
