using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Infrastructure.Documentation
{
    [AttributeUsage(AttributeTargets.Method)]
    public class EndpointDocumentationAttribute : Attribute
    {
        public string Key { get; }
        public EndpointDocumentationAttribute(string key) => Key = key;
    }

    public record ResponseInfo(int Code, string Description);

}