using Microsoft.Extensions.Configuration;
using Yolva.Bitrix.Client;
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
                        //.AddFilter("ID", "27730")
                        .AddFilter("%NAME", "Кон")
                        .AddFilter("%LAST_NAME", "Ба")
                        .AddSelect("NAME", "EMAIL");
            var result = client.RetrieveListAsync<BitrixResponse<CrmContact>>(query).GetAwaiter().GetResult();

        }
    }
}