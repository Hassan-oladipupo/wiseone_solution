using flexiCoreService.MessageHandler;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;

namespace flexiCoreService
{
    [HubName("messageHub")]
    public class MessageHub : Hub<IMessageCallbacks>, IMessageCalls
    {
        public async Task AddNote(string note)
        {
            await Clients.All.BroadcastNewNote(note);
        }
    }
}