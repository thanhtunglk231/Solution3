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

        // Map username -> connectionId
        private static readonly ConcurrentDictionary<string, string> _userConnections = new();

        public ChatHub(ICChat chatMessageService)
        {
            _chatMessageService = chatMessageService;
        }

        public override Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var username = httpContext?.Request?.Query["username"];

            if (!string.IsNullOrEmpty(username))
            {
                _userConnections[username] = Context.ConnectionId;
                Console.WriteLine($"[SignalR] ✅ Map '{username}' -> {Context.ConnectionId}");
            }

            Console.WriteLine($"[SignalR] Connected: {Context.ConnectionId}");
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            var username = GetUsernameByConnectionId(Context.ConnectionId);
            if (username != null)
            {
                _userConnections.TryRemove(username, out _);
                Console.WriteLine($"[SignalR] ❌ Unmapped user '{username}'");
            }

            Console.WriteLine($"[SignalR] Disconnected: {Context.ConnectionId}");
            return base.OnDisconnectedAsync(exception);
        }

        private string GetUsernameByConnectionId(string connectionId)
        {
            foreach (var kv in _userConnections)
                if (kv.Value == connectionId) return kv.Key;
            return null;
        }

        public async Task SendMessage(ChatMessageDto dto)
        {
            Console.WriteLine("[DEBUG] Incoming DTO:");
            Console.WriteLine($"Sender: {dto.SenderUsername} | Receiver: {dto.ReceiverUsername} | Group: {dto.GroupId}");
            Console.WriteLine($"Text: {dto.MessageText}");
            Console.WriteLine($"Attachments: {string.Join(", ", dto.AttachmentUrls ?? new())}");

            if (string.IsNullOrWhiteSpace(dto.SenderUsername))
            {
                await Clients.Caller.SendAsync("ReceiveError", "SenderUsername đang bị null");
                return;
            }

            dto.Timestamp ??= DateTime.UtcNow;

            var result = await _chatMessageService.SaveMessageAsync(dto);
            if (!result.success)
            {
                await Clients.Caller.SendAsync("ReceiveError", result.message);
                return;
            }

            if (!string.IsNullOrEmpty(dto.ReceiverUsername))
            {
                // 1-1
                await Clients.Caller.SendAsync("ReceiveMessage", dto);
                if (_userConnections.TryGetValue(dto.ReceiverUsername, out var conn))
                    await Clients.Client(conn).SendAsync("ReceiveMessage", dto);
            }
            else if (!string.IsNullOrEmpty(dto.GroupId))
            {
                // Group
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

        // Fallback: client truyền thêm receiverUsername OR groupId
        public async Task DeleteMessage(string messageId, string receiverUsername, string groupId)
        {
            if (string.IsNullOrWhiteSpace(messageId))
            {
                await Clients.Caller.SendAsync("ReceiveError", "messageId không hợp lệ");
                return;
            }

            var requester = GetUsernameByConnectionId(Context.ConnectionId);
            if (string.IsNullOrEmpty(requester))
            {
                await Clients.Caller.SendAsync("ReceiveError", "Không xác định người yêu cầu xoá");
                return;
            }

            // TODO (khuyến nghị): xác thực requester là chủ sở hữu messageId
            // bằng cách truy DB lấy chủ sở hữu của messageId và so sánh.
            // Ở bản fallback này chưa có hàm service => chấp nhận theo meta client gửi lên.

            var del = await _chatMessageService.Delete_Message(messageId);
            if (!del.Success)
            {
                await Clients.Caller.SendAsync("ReceiveError", del.message ?? "Xoá tin nhắn thất bại");
                return;
            }

            // Thông báo gỡ tin cho caller
            await Clients.Caller.SendAsync("MessageDeleted", messageId);

            // 1-1: báo cho người còn lại nếu đang online
            if (!string.IsNullOrEmpty(receiverUsername))
            {
                if (_userConnections.TryGetValue(receiverUsername, out var conn))
                    await Clients.Client(conn).SendAsync("MessageDeleted", messageId);
            }
            // Group: broadcast cho cả group
            else if (!string.IsNullOrEmpty(groupId))
            {
                await Clients.Group(groupId).SendAsync("MessageDeleted", messageId);
            }

            Console.WriteLine($"[SignalR] 🗑️ MessageDeleted {messageId} by {requester}");
        }
    }
}
