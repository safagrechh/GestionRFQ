using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using System.Collections.Concurrent;

namespace EX.UI.Web.Hubs
{
    [AllowAnonymous]
    public class NotificationHub : Hub
    {
        // Store user connections for easy lookup
        private static readonly ConcurrentDictionary<string, HashSet<string>> UserConnections = new();

        public override async Task OnConnectedAsync()
        {
            var userId = GetUserId();
            if (!string.IsNullOrEmpty(userId))
            {
                // Add connection to user's connection list
                UserConnections.AddOrUpdate(userId, 
                    new HashSet<string> { Context.ConnectionId },
                    (key, connections) => 
                    {
                        connections.Add(Context.ConnectionId);
                        return connections;
                    });

                // Join user to their personal group
                await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}");
                
                Console.WriteLine($"User {userId} connected with connection {Context.ConnectionId}");
            }
            else
            {
                // Allow anonymous connections but log them
                Console.WriteLine($"Anonymous user connected with connection {Context.ConnectionId}");
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userId = GetUserId();
            if (!string.IsNullOrEmpty(userId))
            {
                // Remove connection from user's connection list
                if (UserConnections.TryGetValue(userId, out var connections))
                {
                    connections.Remove(Context.ConnectionId);
                    if (connections.Count == 0)
                    {
                        UserConnections.TryRemove(userId, out _);
                    }
                }

                // Remove from user group
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"User_{userId}");
                
                Console.WriteLine($"User {userId} disconnected from connection {Context.ConnectionId}");
            }
            else
            {
                Console.WriteLine($"Anonymous user disconnected from connection {Context.ConnectionId}");
            }

            await base.OnDisconnectedAsync(exception);
        }

        // Client can join with a specific user ID (for cases where JWT is not available)
        public async Task JoinUserGroup(string userId)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}");
                
                // Update user connections
                UserConnections.AddOrUpdate(userId, 
                    new HashSet<string> { Context.ConnectionId },
                    (key, connections) => 
                    {
                        connections.Add(Context.ConnectionId);
                        return connections;
                    });
                
                Console.WriteLine($"Connection {Context.ConnectionId} joined user group {userId}");
            }
        }

        // Client can leave a user group
        public async Task LeaveUserGroup(string userId)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"User_{userId}");
                
                // Update user connections
                if (UserConnections.TryGetValue(userId, out var connections))
                {
                    connections.Remove(Context.ConnectionId);
                    if (connections.Count == 0)
                    {
                        UserConnections.TryRemove(userId, out _);
                    }
                }
                
                Console.WriteLine($"Connection {Context.ConnectionId} left user group {userId}");
            }
        }

        // Send notification to a specific user
        public async Task SendNotificationToUser(string userId, object notification)
        {
            await Clients.Group($"User_{userId}").SendAsync("ReceiveNotification", notification);
        }

        // Test connection method for frontend debugging
        public async Task TestConnection()
        {
            var userId = GetUserId();
            var connectionId = Context.ConnectionId;
            var isConnected = IsUserConnected(userId ?? "anonymous");
            
            await Clients.Caller.SendAsync("TestConnectionResponse", new
            {
                UserId = userId,
                ConnectionId = connectionId,
                IsConnected = isConnected,
                Message = $"Test successful! User {userId ?? "anonymous"} is connected.",
                Timestamp = DateTime.UtcNow
            });
        }

        // Helper method to get user ID from context
        private string GetUserId()
        {
            // Try to get from JWT token claims - handle multiple NameIdentifier claims
            var nameIdentifierClaims = Context.User?.FindAll(ClaimTypes.NameIdentifier);
            if (nameIdentifierClaims != null)
            {
                var userIdClaim = nameIdentifierClaims.FirstOrDefault(c => int.TryParse(c.Value, out _))?.Value;
                if (!string.IsNullOrEmpty(userIdClaim))
                {
                    return userIdClaim;
                }
            }

            // Try alternative claim types
            var subClaim = Context.User?.FindFirst("sub")?.Value;
            if (!string.IsNullOrEmpty(subClaim))
            {
                return subClaim;
            }

            // Try from query string (fallback for cases where JWT is not available)
            var queryUserId = Context.GetHttpContext()?.Request.Query["userId"].FirstOrDefault();
            if (!string.IsNullOrEmpty(queryUserId))
            {
                return queryUserId;
            }

            return null;
        }

        // Static method to get user connections (for external use)
        public static HashSet<string> GetUserConnections(string userId)
        {
            return UserConnections.TryGetValue(userId, out var connections) ? connections : new HashSet<string>();
        }

        // Static method to get connected users count
        public static int GetConnectedUsersCount()
        {
            return UserConnections.Count;
        }

        // Static method to check if user is connected
        public static bool IsUserConnected(string userId)
        {
            return UserConnections.ContainsKey(userId) && UserConnections[userId].Count > 0;
        }
    }
}
