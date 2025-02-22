using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.Services
{
    public class Notification : Hub
    {
        public async Task SendMessage(string messgae) 
        {
            await Clients.All.SendAsync("ReceiveMessage", messgae);
        }
    }
}