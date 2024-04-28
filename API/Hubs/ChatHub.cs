using API.Model.Dtos.UserConnection;
using API.Services.Common;
using Microsoft.AspNetCore.SignalR;

namespace API.Hubs
{
    public class ChatHub : Hub
    {
        private readonly SharedDb _shared;

        public ChatHub(SharedDb shared)
        {
            _shared = shared;
        }
        public async Task JoinChat(UserConnection connection)
        {
            await Clients.All.SendAsync("ReceiveMessage", "admin", $"{connection.Username} has joined");
        }

        public async Task JoinSpecificChatRoom(UserConnection connection)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, connection.ChatRoom);

            _shared.connections[Context.ConnectionId] = connection;

            //await Clients.Group(connection.ChatRoom).SendAsync("ReceiveMessage", "admin", $"{connection.Username} kết nối chat với {connection.ChatRoom}");
        }

        public async Task SendMessage(string msg)
        {
            if (_shared.connections.TryGetValue(Context.ConnectionId, out UserConnection connection))
            {
                await Clients.Group(connection.ChatRoom).SendAsync("ReceiveSpecificMessage", connection.Username, msg);
            }
        }

        public async Task SendPrivateMessage(string recipient, string message)
        {
            var recipientConnectionId = _shared.connections.FirstOrDefault(x => x.Value.Username == recipient).Key;
            if (!string.IsNullOrEmpty(recipientConnectionId))
            {
                await Clients.Client(recipientConnectionId).SendAsync("ReceiveSpecificMessage", "Admin", message);
            }
        }

    }
}
