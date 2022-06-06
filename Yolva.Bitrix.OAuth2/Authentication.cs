using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Yolva.Bitrix.OAuth2
{
    public sealed class Authentication
    {
        private Site24 site;
        private NetworkCredential credential;
        private string clientId;
        private string client_secret;
        private string grant_type;

        public Authentication(Site24 site,NetworkCredential credential,string clientId,string client_secret,string grant_type)
        {
            this.site = site;
            this.credential = credential;
            this.clientId = clientId;
            this.client_secret = client_secret;
            this.grant_type = grant_type;
        }

        private string getSessionId()
        {
            using var session = new Authorization(site, credential);
            return session.GetSessionId();
        }
        private string getCodeId(string sessionId)
        {
            using var code = new Code(clientId, sessionId, site);
            var codeId = code.GetCode();
            return codeId;
        }
        private async Task<string> getTokenContent(string codeIdOrRefreshToken,bool isRefreshToken)
        {
            using var token = new Token(grant_type, clientId, client_secret, codeIdOrRefreshToken);
            var tokenObj = await token.GetAuthContent(isRefreshToken);
            return tokenObj;
        }

        public string GetTokenContent(string? refreshToken = null)
        {
            if (refreshToken == null)
            {
                var sessionId = getSessionId();
                var codeId = getCodeId(sessionId);
                return getTokenContent(codeId, false).Result;
            }
            else
            {
                grant_type = Variables.grantTypeRefreshTokenName;
                return getTokenContent(refreshToken, true).Result;
            }
        }
    }
}
