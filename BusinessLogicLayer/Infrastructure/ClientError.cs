using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Infrastructure
{
    public class ClientError : Exception
    {
        public int StatusCode { get; }
        public ClientError(int statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }

        public ClientError()
        {
        }
    }
}
