using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yolva.Bitrix.OAuth2
{
    public sealed class Site24
    {
        public Uri Host { get; }
        public Site24(string host)
        {
            if(String.IsNullOrEmpty(host.Trim()))
                throw new ArgumentNullException(nameof(host));
            if (!Uri.IsWellFormedUriString(host, UriKind.RelativeOrAbsolute))
                throw new Exception($"\"{host}\" - invalid format");
            this.Host = new Uri(host);
        }
    }
}
