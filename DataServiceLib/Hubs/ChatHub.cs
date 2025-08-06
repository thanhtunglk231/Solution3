using CoreLib.Dtos;
using DataServiceLib.Interfaces1;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace DataServiceLib.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ICChat _chatMessageService;

        // Lưu ánh xạ giữa username và connectionId
        private static readonly ConcurrentDictionary<string, string> _userConnections = new();

        public ChatHub(ICChat chatMessageService)
        {
            _chatMessageService = chatMessageService;
        }

        public override Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var username = httpContext.Request.Query["username"];

            if (!string.IsNullOrEmpty(username))
            {
                _userConnections[username] = Context.ConnectionId;
                Console.WriteLine($"[SignalR] ✅ Gán username '{username}' với ConnectionId '{Context.ConnectionId}'");
            }

            Console.WriteLine($"[SignalR] Client connected: {Context.ConnectionId}");
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            var username = GetUsernameByConnectionId(Context.ConnectionId);
            if (username != null)
            {
                _userConnections.TryRemove(username, out _);
                Console.WriteLine($"[SignalR] ❌ User '{username}' disconnected, removed from mapping.");
            }

            Console.WriteLine($"[SignalR] Client disconnected: {Context.ConnectionId}");
            return base.OnDisconnectedAsync(exception);
        }

        private string GetUsernameByConnectionId(string connectionId)
        {
            foreach (var pair in _userConnections)
            {
                if (pair.Value == connectionId)
                    return pair.Key;
            }
            return null;
        }

        public async Task SendMessage(ChatMessageDto dto)
        {
            Console.WriteLine("[DEBUG] Nhận DTO:");
            Console.WriteLine($"SenderUsername: {dto.SenderUsername}");
            Console.WriteLine($"ReceiverUsername: {dto.ReceiverUsername}");
            Console.WriteLine($"GroupId: {dto.GroupId}");
            Console.WriteLine($"MessageText: {dto.MessageText}");

            if (string.IsNullOrWhiteSpace(dto.SenderUsername))
            {
                await Clients.Caller.SendAsync("ReceiveError", "SenderUsername đang bị null");
                return;
            }

            var result = await _chatMessageService.SaveMessageAsync(dto);

            if (!result.success)
            {
                await Clients.Caller.SendAsync("ReceiveError", result.message);
                return;
            }

            // Gửi về cho người nhận nếu là tin nhắn riêng
            if (!string.IsNullOrEmpty(dto.ReceiverUsername))
            {
                // Gửi cho người gửi
                await Clients.Caller.SendAsync("ReceiveMessage", dto);

                // Gửi cho người nhận (nếu họ đang online)
                if (_userConnections.TryGetValue(dto.ReceiverUsername, out var receiverConnId))
                {
                    await Clients.Client(receiverConnId).SendAsync("ReceiveMessage", dto);
                }
            }
            else if (!string.IsNullOrEmpty(dto.GroupId))
            {
                // Gửi đến tất cả thành viên nhóm
                await Clients.Group(dto.GroupId).SendAsync("ReceiveMessage", dto);
            }
        }

        public async Task JoinGroup(string groupId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupId);
        }

        public async Task LeaveGroup(string groupId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupId);
        }
    }
}
