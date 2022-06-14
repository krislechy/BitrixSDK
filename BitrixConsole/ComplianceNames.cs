using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitrixConsole
{
    internal class ComplianceNames
    {
        private string path { get; set; }
        private Dictionary<string, string> dict;
        public ComplianceNames(string path)
        {
            this.path = path;
            var e = File.ReadAllText(path);
            var json = JsonConvert.DeserializeObject<Dictionary<string, string>>(e);
            dict = new Dictionary<string, string>(json);
        }

        public string? this[string str]
        {
            get
            {
                return dict.TryGetValue(str, out var result) ? result : null;
            }
        }
    }
}
