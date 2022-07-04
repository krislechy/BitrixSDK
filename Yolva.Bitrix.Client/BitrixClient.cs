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
using Yolva.Bitrix.Client.Abstractions;
using Yolva.Bitrix.Client.Entities;
using Yolva.Bitrix.Extensions;
using Yolva.Bitrix.OAuth2;

namespace Yolva.Bitrix.Client
{
    public class BitrixClient : BaseClient, IBitrixService
    {
        #region TokenRepresent
        [TokenRepresent, DefaultValue(null)]
        public string? access_token { get; private set; }
        [TokenRepresent, DefaultValue(null)]
        public int? expires { get; private set; }
        [TokenRepresent, DefaultValue(null)]
        public int? expires_in { get; private set; }
        [TokenRepresent, DefaultValue(null)]
        public string? scope { get; private set; }
        [TokenRepresent, DefaultValue(null)]
        public string? domain { get; private set; }
        [TokenRepresent, DefaultValue(null)]
        public string? server_endpoint { get; private set; }
        [TokenRepresent, DefaultValue(null)]
        public string? status { get; private set; }
        [TokenRepresent, DefaultValue(null)]
        public string? client_endpoint { get; private set; }
        [TokenRepresent, DefaultValue(null)]
        public string? member_id { get; private set; }
        [TokenRepresent, DefaultValue(null)]
        public int? user_id { get; private set; }
        [TokenRepresent, DefaultValue(null)]
        public string? refresh_token { get; private set; }
        #endregion
        public AuthParameters AuthParam { get; }
        private Authentication authentication;
        private object sync = new object();
        public bool isExpired
        {
            get
            {
                lock (sync)
                {
                    var nowDt = DateTime.Now;
                    var _expires = expires.TimeStampToDateTime();
                    return (nowDt - _expires).TotalSeconds >= 0 - TimeSpan.FromMinutes(5).TotalSeconds;
                }
            }
        }
        public BitrixClient(AuthParameters authParam)
        {
            this.AuthParam = authParam;
            Connect();
        }
        private void checkLifeTimeToken()
        {
            if (isExpired)
                Connect(refresh_token);
        }
        private void Connect(string? refreshToken = null)
        {
            authentication ??=
                new Authentication(AuthParam.site, AuthParam.credential, AuthParam.clientId, AuthParam.client_secret, AuthParam.grant_type);
            var tokenObj = authentication.GetTokenContent(refreshToken);
            if (refreshToken != null)
                clearOAuthProperties(this);
            applyAuthObj(tokenObj, this);
            if (access_token != null && !isExpired)
            {
                if (refreshToken == null)
                    Trace.WriteLine($"Established connection to {AuthParam.site.Host}");
                else
                    Trace.WriteLine($"Refreshed token for {AuthParam.site.Host}");
            }
        }
        #region Service
        public async Task<T?> RetrieveListAsync<T>(Bitrix24QueryBuilder query)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));
            checkCommand(query.Command, "list","fields");
            checkLifeTimeToken();
            var response = await CreateRequest(AuthParam.site.Host, query.Command, query.Create(), access_token);
            var content = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var entities = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(content);
                return entities;
            }
            else throw new UnexpectedStatusCodeException(HttpStatusCode.OK, response.StatusCode, getStringJToken(content));
        }

        public async Task<TResponse?> CreateAsync<TEntity, TResponse>(string command, TEntity? entity, long? id = null)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            checkCommand(command, "add","bind","unbind");
            checkLifeTimeToken();
            var requestContent = new RequestContentBitrix<TEntity>()
            {
                id = id,
                fields = entity
            };
            var response = await CreateRequest(AuthParam.site.Host, command, requestContent, access_token);
            var content = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                string resultToken = (string)JToken.Parse(content)["result"];
                return resultToken.Convert<TResponse>();
            }
            else throw new UnexpectedStatusCodeException(HttpStatusCode.OK, response.StatusCode, getStringJToken(content));
        }
        public async Task UpdateAsync<TEntity>(string command, TEntity? entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            checkCommand(command, "update");
            checkLifeTimeToken();
            var id = checkRequiredProperty<long>(entity, "ID");
            var requestContent = new RequestContentBitrix<TEntity>()
            {
                id = id,
                fields = entity
            };
            var response = await CreateRequest(AuthParam.site.Host, command, requestContent, access_token);
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                throw new UnexpectedStatusCodeException(HttpStatusCode.OK, response.StatusCode, getStringJToken(content));
            }
        }
        public async Task DeleteAsync(string command, long id)
        {
            checkCommand(command, "delete");
            checkLifeTimeToken();
            if (equalGeneric(id, default(long))) throw new Exception("Id cannot be null or zero");
            var requestContent = "{" + $"\"id\":{id}" + "}";
            var response = await CreateRequest(AuthParam.site.Host, command, requestContent, access_token);
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                throw new UnexpectedStatusCodeException(HttpStatusCode.OK, response.StatusCode, getStringJToken(content));
            }
        }
        #endregion
    }
}