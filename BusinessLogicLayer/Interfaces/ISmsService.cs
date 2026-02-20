using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Interfaces
{
    public interface ISmsService
    {
        Task SendSmsAsync(string toPhoneNumber, string message);
    }
}
