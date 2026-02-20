using BusinessLogicLayer.Infrastructure.Documentation.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Infrastructure.Documentation
{
    public static class DocStore
    {
        public static readonly Dictionary<string, (string Summary, string Remarks, List<ResponseInfo> Responses)> AllEndpoints;

        static DocStore()
        {
            // Find all classes that implement IDocDefinition
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            AllEndpoints = assemblies
                .SelectMany(s => s.GetTypes())
                .Where(p => typeof(IDocDefinition).IsAssignableFrom(p) && !p.IsInterface && !p.IsAbstract)
                .Select(t => (IDocDefinition)Activator.CreateInstance(t)!)
                .SelectMany(d => d.GetDocs())
                .ToDictionary(pair => pair.Key, pair => pair.Value);
        }
    }
}
