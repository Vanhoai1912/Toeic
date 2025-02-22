using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace ToeicWeb.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string sender, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", sender, message);
        }
    }
}