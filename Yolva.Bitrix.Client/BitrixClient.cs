using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Yolva.Bitrix.Extensions;
using Yolva.Bitrix.OAuth2;

namespace Yolva.Bitrix.Client
{
    public class BitrixClient: IBitrixService
    {
        public AuthParameters AuthParam { get; }
        private Authentication authentication;
        #region ResponseJsonToken
        [OAuthResponseAttribute, DefaultValue(null)]
        public string? access_token { get; private set; }
        [OAuthResponseAttribute, DefaultValue(null)]
        public int? expires { get; private set; }
        [OAuthResponseAttribute, DefaultValue(null)]
        public int? expires_in { get; private set; }
        [OAuthResponseAttribute, DefaultValue(null)]
        public string? scope { get; private set; }
        [OAuthResponseAttribute, DefaultValue(null)]
        public string? domain { get; private set; }
        [OAuthResponseAttribute, DefaultValue(null)]
        public string? server_endpoint { get; private set; }
        [OAuthResponseAttribute, DefaultValue(null)]
        public string? status { get; private set; }
        [OAuthResponseAttribute, DefaultValue(null)]
        public string? client_endpoint { get; private set; }
        [OAuthResponseAttribute, DefaultValue(null)]
        public string? member_id { get; private set; }
        [OAuthResponseAttribute, DefaultValue(null)]
        public int? user_id { get; private set; }
        [OAuthResponseAttribute,DefaultValue(null)]
        public string? refresh_token { get; private set; }
        #endregion
        public bool isExpired
        {
            get
            {
                var nowDt = DateTime.Now;
                var _expires = expires.TimeStampToDateTime();
                return (nowDt - _expires).TotalSeconds >= 0;
            }
        }
        private readonly HttpClient client;
        public BitrixClient(AuthParameters authParam)
        {
            this.AuthParam = authParam;
            Connect();
            this.client = new HttpClient();
        }
        #region ClientForming
        private void applyAuthObj(string tokenObj)
        {
            var jtoken = JToken.Parse(tokenObj);
            var typeCurrentInstance = this.GetType();
            var setValueMethod = typeCurrentInstance.GetMethod(nameof(setValue), BindingFlags.NonPublic | BindingFlags.Instance);
            var properties = getOAuthProperties();
            foreach (var property in properties)
            {
                var genericMethod = setValueMethod?.MakeGenericMethod(property.PropertyType);
                genericMethod?.Invoke(this, new object[] { jtoken, property.Name });
            }
        }
        private void clearOAuthProperties()
        {
            var properties = getOAuthProperties();
            var typeCurrentInstance = this.GetType();
            foreach (var property in properties)
            { 
                typeCurrentInstance.
                    InvokeMember(
                    property.Name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.SetProperty | BindingFlags.Instance, 
                    null, this, new object[] { null });
            }
        }
        private void setValue<T>(JToken jt, string propertyName)
        {
            if (jt != null)
            {
                var jv = jt[propertyName];
                if (jv != null)
                {
                    var value = jv.Value<T>();
                    setProperty<T>(propertyName, value);
                }
            }
        }
        private void setProperty<T>(string propertyName, T? value)
        {
            var property = this.GetType().GetProperty(propertyName);
            if (value is T)
                property?.SetValue(this, value);
            else throw new Exception($"Cannot cast {(value==null?"null":value)} to {nameof(T)}");
        }
        private IEnumerable<PropertyDescriptor> getOAuthProperties() =>
            TypeDescriptor.GetProperties(this)
              .Cast<PropertyDescriptor>()
              .Where(x => x.Attributes.OfType<OAuthResponseAttribute>().Any());
        private void checkLifeTimeToken(HttpResponseMessage? message = null)
        {
            if (isExpired | message?.StatusCode == HttpStatusCode.Unauthorized)
                Connect(refresh_token);
        }
        private void Connect(string? refreshToken = null)
        {
            authentication ??=
                new Authentication(AuthParam.site, AuthParam.credential, AuthParam.clientId, AuthParam.client_secret, AuthParam.grant_type);
            var tokenObj = authentication.GetTokenContent(refreshToken);
            if (refreshToken != null)
                clearOAuthProperties();
            applyAuthObj(tokenObj);
            if (access_token != null && !isExpired)
                Trace.WriteLine($"Established connection to {AuthParam.site.Host}");
        }
        #endregion
        #region Service
        public async Task<T?> RetrieveListAsync<T>(Bitrix24QueryBuilder query)
        {
            if (query==null) throw new ArgumentNullException(nameof(query));
            if (String.IsNullOrEmpty(query.Command?.Trim())) throw new ArgumentNullException(nameof(query.Command));
            var sCom = query.Command.Split(".");
            if (sCom.Last().ToLower() != "list")
                throw new Exception($"Command should be end on \"list\"");

            var jsonRequest = new StringContent(query.Create(), Encoding.UTF8, "application/json");
            var get = await client.PostAsync($"{AuthParam.site.Host}rest/{query.Command}?auth={access_token}", jsonRequest);
            var content = await get.Content.ReadAsStringAsync();
            var entities = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(content);
            return entities;
        }
        #endregion
    }
}