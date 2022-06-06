using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Yolva.Bitrix.Extensions;

namespace Yolva.Bitrix.OAuth2
{
    internal sealed class Authorization:IDisposable
    {
        private readonly string login;
        private readonly string password;
        private readonly HttpClient client;
        private readonly Site24 site;
        private readonly CookieContainer cookie;
        private readonly Dictionary<string, string> defaultParams = new() {
            { "login", "yes"},
        };
        public Authorization(Site24 site, NetworkCredential credentials)
        {
            this.login = credentials.UserName;
            this.password = credentials.Password;
            this.site = site;
            this.cookie = new CookieContainer();
            HttpClientHandler handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false,
                CookieContainer = cookie,
                UseCookies = true,
            };
            client = new HttpClient(handler);
        }
        private FormUrlEncodedContent formData()
        {
            return new FormUrlEncodedContent(
                new Dictionary<string, string>()
            {
                { "AUTH_FORM", "Y"},
                { "TYPE", "AUTH"},
                { "USER_LOGIN", login},
                { "USER_PASSWORD", password},
            });
        }
        private async Task<HttpResponseMessage> sendRequest() =>
            await client.PostAsync($"{site.Host}auth/?" + defaultParams.ToParams(), formData());
        public string GetSessionId()
        {
            var response = sendRequest().Result;
            if (response.IsSuccessStatusCode)
            {
                var cookieHost = cookie.GetCookies(site.Host);
                if (cookieHost.Any(x => x.Name == Variables.PHPSESSIDName))
                {
                    var sessionId = cookieHost[Variables.PHPSESSIDName];
                    if (sessionId != null)
                        return sessionId.Value;
                    else throw new ArgumentNullException($"{Variables.PHPSESSIDName} is NULL");
                }
                else throw new ArgumentNullException($"Expected {Variables.PHPSESSIDName} in Cookies");
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
