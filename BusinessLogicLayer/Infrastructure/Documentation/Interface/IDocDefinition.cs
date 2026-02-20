using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Infrastructure.Documentation.Interface
{
    public interface IDocDefinition
    {
        // A dictionary of Key -> (Summary, Remarks, Responses)
        Dictionary<string, (string Summary, string Remarks, List<ResponseInfo> Responses)> GetDocs();
    }
}
