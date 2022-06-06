using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Yolva.Bitrix.OAuth2;

namespace Yolva.Bitrix.Client
{
    public sealed class AuthParameters
    {
        public Site24 site { get; }
        public NetworkCredential credential { get; }
        public string clientId { get; }
        public string client_secret { get; }
        public string grant_type { get; } = "authorization_code";
        public AuthParameters(Site24 site, NetworkCredential credential, string clientId, string client_secret, string? grant_type = null)
        {
            this.site = site;
            this.credential = credential;
            this.clientId = clientId;
            this.client_secret = client_secret;
            if (grant_type != null)
                this.grant_type = grant_type;
        }
    }
}
