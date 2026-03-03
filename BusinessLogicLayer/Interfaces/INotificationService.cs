using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Interfaces
{
    public interface INotificationService
    {
        Task NotifyUserStatusChange(string userId, bool isDeactivated, string message);
        Task SendGlobalNotification(string title, string body);
    }
}
