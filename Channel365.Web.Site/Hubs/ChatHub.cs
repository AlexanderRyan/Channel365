using Microsoft.AspNetCore.SignalR;

namespace Channel365.Web.Hubs {
    public class ChatHub : Hub
    {
        public string GetConnectionId() =>
           Context.ConnectionId;
    }
}
