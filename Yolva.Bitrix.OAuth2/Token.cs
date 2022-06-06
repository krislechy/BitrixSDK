using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yolva.Bitrix.Extensions;

namespace Yolva.Bitrix.OAuth2
{
    internal sealed class Token:IDisposable
    {
        private readonly string grantType;
        private readonly string client_id;
        private readonly string client_secret;
        private readonly string codeOrRefreshToken;
        private readonly HttpClient client;
        private readonly Uri OAuthUri = new Uri("https://oauth.bitrix.info");
        public Token(string grantType, string client_id, string client_secret, string codeOrRefreshToken, Uri? _OAuthUri = null)
        {
            this.grantType = grantType;
            this.client_id = client_id;
            this.client_secret = client_secret;
            this.codeOrRefreshToken = codeOrRefreshToken;
            if (_OAuthUri != null)
                this.OAuthUri = _OAuthUri;
            this.client = new HttpClient();
        }

        private Dictionary<string, string> paramsToken() => new Dictionary<string, string>()
        {
            {Variables.grantTypeName,grantType },
            {Variables.clientIdName,client_id },
            {Variables.clientSecretName,client_secret},
            {Variables.codeQueryName,codeOrRefreshToken },
        };
        private async Task<HttpResponseMessage> sendRequest(Dictionary<string, string> _params) => 
            await client.GetAsync($"{OAuthUri}oauth/token/?{_params.ToParams()}");

        public async Task<string> GetAuthContent(bool isRefreshToken)
        {
            Dictionary<string, string> _params = paramsToken();
            if (isRefreshToken)
            {
                if (grantType != Variables.grantTypeRefreshTokenName) 
                    throw new InvalidDataException($"{Variables.grantTypeName} must to be \"{Variables.grantTypeRefreshTokenName}\"");
                _params.Remove(Variables.codeQueryName);
                _params.Remove(Variables.refreshTokenName);
                _params.Add(Variables.refreshTokenName, codeOrRefreshToken);
            }
            else
            {
                if (grantType != Variables.grantTypeAuthorizationCodeName) 
                    throw new InvalidDataException($"{Variables.grantTypeName} must to be \"{Variables.grantTypeAuthorizationCodeName}\"");
            }
            var response = sendRequest(_params).Result;
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                if (content != null) return content;
                else throw new ArgumentNullException("Content Token is null");
            }
            else throw new HttpRequestException($"Expected status code 200, now: {(int)response.StatusCode}");
        }

        public void Dispose()
        {
            client.Dispose();
            var gen = GC.GetGeneration(this);
            GC.Collect(gen, GCCollectionMode.Optimized);
        }
    }
}
