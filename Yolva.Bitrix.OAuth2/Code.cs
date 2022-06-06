using System.Net;
using System.Web;
using Yolva.Bitrix.Extensions;

namespace Yolva.Bitrix.OAuth2
{
    internal sealed class Code:IDisposable
    {
        private readonly string client_id;
        private readonly string state;
        private readonly HttpClient client;
        private readonly CookieContainer cookie;
        private readonly string sessionId;
        private readonly Site24 site;
        public Code(string client_id, string sessionId, Site24 site)
        {
            this.client_id = client_id;
            this.sessionId = sessionId;
            this.site = site;
            this.state = State.GenerateState();
            cookie = new CookieContainer();
            setSession();
            var handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false,
                CookieContainer = cookie,
                UseCookies = true,
            };
            this.client = new HttpClient(handler);
        }
        private void setSession() =>
            cookie.Add(site.Host, new Cookie(Variables.PHPSESSIDName, sessionId));
        private Dictionary<string, string> paramsDict() => new Dictionary<string, string>()
        {
            {Variables.clientIdName,client_id },
            {Variables.stateName,state},
        };
        private async Task<HttpResponseMessage> sendRequest()=>
            await client.GetAsync($"{site.Host}oauth/authorize/?{paramsDict().ToParams()}");

        public string GetCode()
        {
            var response=sendRequest().Result;
            if (response.StatusCode == HttpStatusCode.Found)
            {
                var query = response.Headers?.Location?.Query;
                if (query == null) throw new ArgumentNullException($"Query in headers is null");
                var queryDict = HttpUtility.ParseQueryString(query);
                if (queryDict == null) throw new ArgumentNullException($"Query Dictionary is null");
                var code = queryDict.Get(Variables.codeQueryName);
                if (code != null)
                    return code;
                else throw new ArgumentNullException($"Cannot find \"${Variables.codeQueryName}\" in Query");
            }
            else throw new HttpRequestException($"Expected 302 status code, now: {(int)response.StatusCode}");
        }

        public void Dispose()
        {
            client.Dispose();
            var gen = GC.GetGeneration(this);
            GC.Collect(gen, GCCollectionMode.Optimized);
        }
    }
}