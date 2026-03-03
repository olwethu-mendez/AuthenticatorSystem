using BusinessLogicLayer.Hubs;
using BusinessLogicLayer.Infrastructure;
using BusinessLogicLayer.Interfaces;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }
        public async Task NotifyUserStatusChange(string userId, bool isDeactivated, string message)
        {
            await _hubContext.Clients.User(userId).SendAsync(HubReceiveMethods.AccountStatus, new
            {
                userId,
                isDeactivated,
                message
            });
        }

        public async Task SendGlobalNotification(string title, string body)
        {
            await _hubContext.Clients.All.SendAsync(HubReceiveMethods.Notification, new
            {
                title,
                body,
                timestamp = DateTime.UtcNow 
            });
        }
    }
}
