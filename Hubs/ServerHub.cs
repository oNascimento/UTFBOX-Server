using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace UTFBox_Server.Hubs
{
    public class ServerHub : Hub
    {
        public async Task SendMessage(string message){
            await Clients.All.SendAsync("Mensagem Recebida: " + message);
        }
    }
}