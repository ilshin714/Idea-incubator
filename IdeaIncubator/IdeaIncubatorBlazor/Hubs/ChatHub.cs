using Microsoft.AspNetCore.SignalR;

namespace IdeaIncubatorBlazor.Hubs
{
    public class ChatHub : Hub
    {
        private static Dictionary<string, string> Users = new Dictionary<string, string>();

        public override async Task OnConnectedAsync()
        {
            int groupChatId = Convert.ToInt32(Context.GetHttpContext().Request.Query["groupChatId"]);

            string username = Context.GetHttpContext().Request.Query["username"];
            Users.Add(Context.ConnectionId, username);
            if (username.Contains("1:1_"))
            {
                await AddMessageToChat(username, $"Chat Alarm: {username} joined 1:1 Chat!");
            }
            else
            {
                await AddMessageToChat(username, $"Chat Alarm: {username} joined the party!");
            }
            
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            string username = Users.FirstOrDefault(u => u.Key == Context.ConnectionId).Value;
            if (username.Contains("1:1_"))
            {
                await AddMessageToChat(username, $"Chat Alarm: {username} left 1:1 Chat!");
            }
            else
            {
                await AddMessageToChat(username, $"Chat Alarm: {username} left!");
            }
            
        }

        public async Task AddMessageToChat(string user, string message, int groupChatId = 0)
        {
            await Clients.All.SendAsync("GetMessage", user, message, groupChatId);
        }
    }
}
