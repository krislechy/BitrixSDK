using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Yolva.Bitrix.Client.Abstractions;
using Yolva.Bitrix.Extensions;
using Yolva.Bitrix.OAuth2;

namespace Yolva.Bitrix.Client
{
    public abstract class BaseClient
    {
        protected virtual HttpContent? GetStringContent(object? content)=>
            new StringContent(content is string?(string)content: SerializeObject(content), Encoding.UTF8, "application/json");
        private string SerializeObject(object? content)=>
            JsonConvert.SerializeObject(content, Formatting.None, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
            });
        protected virtual async Task<HttpResponseMessage> CreateRequest(Uri host,string command, object? content,string? access_token)
        {
            if (access_token == null) throw new ArgumentNullException(nameof(access_token));
            var jsonContent = GetStringContent(content);
            using var client = new HttpClient();
            var response = await client.PostAsync($"{host}rest/{command}?auth={access_token}", jsonContent);
            return response;
        }

        #region HelperMethods
        protected PropertyInfo? getProperty(object obj, string propertyName)
        {
            var type = obj.GetType();
            return type.GetProperty(propertyName);
        }
        protected T checkRequireProperty<T>(object obj, string propertyName)
        {
            var property = getProperty(obj, propertyName);
            if (property == null) throw new Exception($"Required property \"{propertyName}\" do not exist");
            var value = (T?)property.GetValue(obj);
            if (equalGeneric(value, default(T))) throw new Exception($"Value {propertyName} (required) is null");
            return (T)value;
        }
        protected bool equalGeneric<T>(T t1, T t2) => EqualityComparer<T>.Default.Equals(t1, t2);
        protected void checkCommand(string command, string expectedEnd)
        {
            if (String.IsNullOrEmpty(command?.Trim())) throw new ArgumentNullException(nameof(command));
            var sCom = command.Split(".");
            if (sCom.Last().ToLower() != expectedEnd)
                throw new Exception($"Command should be end on \"{expectedEnd}\"");
        }
        #endregion
    }
}
