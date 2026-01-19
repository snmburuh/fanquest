using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;

namespace FanQuest.API.Hubs
{
    public class QuestHub : Hub
    {
        private readonly ILogger<QuestHub> _logger;

        public QuestHub(ILogger<QuestHub> logger)
        {
            _logger = logger;
        }

        public async Task JoinQuestGroup(string questId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"quest_{questId}");
            _logger.LogInformation("Client {ConnectionId} joined quest group {QuestId}",
                Context.ConnectionId, questId);
        }

        public async Task LeaveQuestGroup(string questId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"quest_{questId}");
            _logger.LogInformation("Client {ConnectionId} left quest group {QuestId}",
                Context.ConnectionId, questId);
        }

        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation("Client connected: {ConnectionId}", Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _logger.LogInformation("Client disconnected: {ConnectionId}", Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
    }
}
