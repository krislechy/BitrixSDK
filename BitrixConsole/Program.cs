using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Yolva.Bitrix.Client;
using Yolva.Bitrix.Client.Abstractions;
using Yolva.Bitrix.Client.Entities;
using Yolva.Bitrix.Client.Entities.Crm;
using Yolva.Bitrix.Extensions;
using Yolva.Bitrix.OAuth2;

namespace BitrixSDK
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            var configCredential = config.GetSection("Credential");
            var credential = new System.Net.NetworkCredential(configCredential["login"], configCredential["password"]);
            var client = new BitrixClient(
                new AuthParameters(
                    new Site24(config["Url"]),
                    credential,
                    config["ClientId"],
                    config["ClientSecret"])
                ) as IBitrixService;
            var query = new Bitrix24QueryBuilder("crm.contact.list")
                        .Order("DATE_CREATE", OrderEnum.ASC)
                        .AddFilter("ID", "27730")
                        //.AddFilter("%NAME", "Кон")
                        //.AddFilter("%LAST_NAME", "Ба")
                        .AddSelect("NAME", "EMAIL", "UF_CRM_1654007527292");
            var result = client.RetrieveListAsync<BitrixResponse<Gavno>>(query).GetAwaiter().GetResult();

            var test = new CrmContact()
            {
                
                NAME = "loh",
            };

            client.Create("crm.contact.add",test).GetAwaiter().GetResult();
        }
    }

    public class Gavno:CrmContact
    {
        [JsonProperty("UF_CRM_1654007527292")]
        public DateTime? VersionUpload { get; set; }
    }
}