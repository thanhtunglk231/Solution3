using CoreLib.Dtos;
using DataServiceLib.Interfaces1;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace DataServiceLib.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ICChat _chatMessageService;

        /// <summary>
        /// Map username -> connectionId (1 user 1 connection, nếu login lại sẽ ghi đè)
        /// </summary>
        private static readonly ConcurrentDictionary<string, string> _userConnections =
            new(StringComparer.OrdinalIgnoreCase);

        public ChatHub(ICChat chatMessageService)
        {
            _chatMessageService = chatMessageService;
        }

        #region Connection mapping

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var username = httpContext?.Request?.Query["username"].ToString();

            if (!string.IsNullOrWhiteSpace(username))
            {
                _userConnections[username] = Context.ConnectionId;
                Console.WriteLine($"[SignalR] ✅ Map '{username}' -> {Context.ConnectionId}");
            }
            else
            {
                Console.WriteLine($"[SignalR] ⚠️ Kết nối {Context.ConnectionId} không có query username");
            }

            Console.WriteLine($"[SignalR] Connected: {Context.ConnectionId}");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var username = GetUsernameByConnectionId(Context.ConnectionId);
            if (username != null)
            {
                _userConnections.TryRemove(username, out _);
                Console.WriteLine($"[SignalR] ❌ Unmapped user '{username}'");
            }

            Console.WriteLine($"[SignalR] Disconnected: {Context.ConnectionId} (error: {exception?.Message})");
            await base.OnDisconnectedAsync(exception);
        }

        private string GetUsernameByConnectionId(string connectionId)
        {
            foreach (var kv in _userConnections)
            {
                if (kv.Value == connectionId)
                    return kv.Key;
            }
            return null;
        }

        #endregion

        #region Send / Receive

        /// <summary>
        /// Gửi tin nhắn 1-1 hoặc nhóm.
        /// - 1-1: dto.ReceiverUsername != null, dto.GroupId == null
        /// - Group: dto.GroupId != null, dto.ReceiverUsername == null
        /// </summary>
        public async Task SendMessage(ChatMessageDto dto)
        {
            Console.WriteLine("[SignalR] >>> SendMessage");
            Console.WriteLine($"  Sender   : {dto?.SenderUsername}");
            Console.WriteLine($"  Receiver : {dto?.ReceiverUsername}");
            Console.WriteLine($"  Group    : {dto?.GroupId}");
            Console.WriteLine($"  Text     : {dto?.MessageText}");
            Console.WriteLine($"  Attach   : {string.Join(", ", dto?.AttachmentUrls ?? Enumerable.Empty<string>())}");

            if (dto == null)
            {
                await Clients.Caller.SendAsync("ReceiveError", "Payload tin nhắn null.");
                return;
            }

            if (string.IsNullOrWhiteSpace(dto.SenderUsername))
            {
                await Clients.Caller.SendAsync("ReceiveError", "SenderUsername đang bị null hoặc rỗng.");
                return;
            }

            var hasText = !string.IsNullOrWhiteSpace(dto.MessageText);
            var hasAttachment = dto.AttachmentUrls != null && dto.AttachmentUrls.Any();

            if (!hasText && !hasAttachment)
            {
                await Clients.Caller.SendAsync("ReceiveError", "Tin nhắn trống (không có nội dung và file đính kèm).");
                return;
            }

            var isPrivate = !string.IsNullOrWhiteSpace(dto.ReceiverUsername);
            var isGroup = !string.IsNullOrWhiteSpace(dto.GroupId);

            if (!isPrivate && !isGroup)
            {
                await Clients.Caller.SendAsync("ReceiveError", "Không xác định được người nhận (thiếu receiverUsername hoặc groupId).");
                return;
            }

            if (isPrivate && isGroup)
            {
                await Clients.Caller.SendAsync("ReceiveError", "Tin nhắn chỉ được gửi 1-1 hoặc nhóm, không thể vừa có receiverUsername vừa có groupId.");
                return;
            }

            // Chuẩn hóa thời gian nếu client không gửi
            dto.Timestamp ??= DateTime.UtcNow;

            // Lưu DB
            var saveResult = await _chatMessageService.SaveMessageAsync(dto);
            if (!saveResult.success)
            {
                await Clients.Caller.SendAsync("ReceiveError", saveResult.message ?? "Lưu tin nhắn thất bại.");
                return;
            }

            // 1-1
            if (isPrivate)
            {
                // Gửi lại cho người gửi (caller)
                await Clients.Caller.SendAsync("ReceiveMessage", dto);

                // Nếu người nhận đang online -> gửi cho connection tương ứng
                if (_userConnections.TryGetValue(dto.ReceiverUsername, out var connId))
                {
                    Console.WriteLine($"[SignalR] 🔔 Send 1-1 to {dto.ReceiverUsername} ({connId})");
                    await Clients.Client(connId).SendAsync("ReceiveMessage", dto);
                }
                else
                {
                    Console.WriteLine($"[SignalR] ℹ️ Receiver '{dto.ReceiverUsername}' hiện offline (không có connectionId).");
                }
            }
            // Nhóm
            else if (isGroup)
            {
                Console.WriteLine($"[SignalR] 📢 Broadcast group {dto.GroupId}");
                await Clients.Group(dto.GroupId).SendAsync("ReceiveMessage", dto);
            }
        }

        public async Task JoinGroup(string groupId)
        {
            if (string.IsNullOrWhiteSpace(groupId))
            {
                await Clients.Caller.SendAsync("ReceiveError", "groupId không hợp lệ khi JoinGroup.");
                return;
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, groupId);
            Console.WriteLine($"[SignalR] 👥 Connection {Context.ConnectionId} joined group {groupId}");
        }

        public async Task LeaveGroup(string groupId)
        {
            if (string.IsNullOrWhiteSpace(groupId))
            {
                await Clients.Caller.SendAsync("ReceiveError", "groupId không hợp lệ khi LeaveGroup.");
                return;
            }

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupId);
            Console.WriteLine($"[SignalR] 👥 Connection {Context.ConnectionId} left group {groupId}");
        }

        #endregion

        #region Delete message

        /// <summary>
        /// Fallback: client truyền thêm receiverUsername OR groupId.
        /// </summary>
        public async Task DeleteMessage(string messageId, string receiverUsername, string groupId)
        {
            if (string.IsNullOrWhiteSpace(messageId))
            {
                await Clients.Caller.SendAsync("ReceiveError", "messageId không hợp lệ.");
                return;
            }

            var requester = GetUsernameByConnectionId(Context.ConnectionId);
            if (string.IsNullOrEmpty(requester))
            {
                await Clients.Caller.SendAsync("ReceiveError", "Không xác định người yêu cầu xoá.");
                return;
            }

            // TODO: kiểm tra quyền sở hữu messageId (nên thêm vào service)
            var del = await _chatMessageService.Delete_Message(messageId);
            if (!del.Success)
            {
                await Clients.Caller.SendAsync("ReceiveError", del.message ?? "Xoá tin nhắn thất bại.");
                return;
            }

            // Thông báo gỡ tin cho caller
            await Clients.Caller.SendAsync("MessageDeleted", messageId);

            // 1-1: báo cho người còn lại nếu đang online
            if (!string.IsNullOrWhiteSpace(receiverUsername))
            {
                if (_userConnections.TryGetValue(receiverUsername, out var conn))
                {
                    await Clients.Client(conn).SendAsync("MessageDeleted", messageId);
                }
            }
            // Group: broadcast cho cả group
            else if (!string.IsNullOrWhiteSpace(groupId))
            {
                await Clients.Group(groupId).SendAsync("MessageDeleted", messageId);
            }

            Console.WriteLine($"[SignalR] 🗑️ MessageDeleted {messageId} by {requester}");
        }

        #endregion
    }
}
