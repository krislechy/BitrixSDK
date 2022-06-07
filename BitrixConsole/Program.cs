using BitrixConsole.Models.Excel;
using Microsoft.Extensions.Configuration;
using Yolva.Bitrix.Client;
using Yolva.Bitrix.Client.Abstractions;
using Yolva.Bitrix.Client.Entities;
using Yolva.Bitrix.Client.Entities.Crm;
using Yolva.Bitrix.Extensions;
using Yolva.Bitrix.OAuth2;

namespace BitrixConsole
{
    public sealed class CachedClients<T> where T : class
    {
        public class CC<T>
        {
            public T client { get; set; }
            public int countUsed { get; set; }=1;
        }
        List<CC<T>> lazy;
        Func<T> funcNewClient;
        public CachedClients(Func<T> func)
        {
            lazy = new();
            funcNewClient = func;
        }
        object sync=new object();
        private T createClient()
        {
            var cl = funcNewClient();
            lazy.Add(new CC<T> { client= cl });
            return cl;
        }
        public T getClient()
        {
            lock (sync)
            {
                foreach (var s in lazy)
                {
                    if (s.countUsed <= 3)
                    {
                        s.countUsed++;
                        return s.client;
                    }
                }
                return createClient();
            }
        }
        public void MarkAsFree(T client)
        {
            lock(sync)
            foreach (var s in lazy)
                if (s.client == client)
                    s.countUsed--;
        }
    }
    public class Program
    {
        private static IConfigurationRoot configuration;
        private static CachedClients<IBitrixService> cachedClients;

        static Program()
        {
            configuration = new ConfigurationBuilder()
                        .AddJsonFile("appsettings.json")
                        .Build();
            cachedClients = new CachedClients<IBitrixService>(()=>BitrixCreateClient());
        }

        public static void Main(string[] args)
        {
            Start().GetAwaiter().GetResult();
        }

        public static IBitrixService BitrixCreateClient()
        {

            var configCredential = configuration.GetSection("Credential");
            var credential = new System.Net.NetworkCredential(configCredential["login"], configCredential["password"]);
            return new BitrixClient(
                new AuthParameters(
                    new Site24(configuration["Url"]),
                    credential,
                    configuration["ClientId"],
                    configuration["ClientSecret"])
                ) as IBitrixService;
            #region test
            //var query = new Bitrix24QueryBuilder("crm.contact.list")
            //            .Order("DATE_CREATE", OrderEnum.ASC)
            //            .AddFilter("ID", "27730")
            //            //.AddFilter("%NAME", "Кон")
            //            //.AddFilter("%LAST_NAME", "Ба")
            //            .AddSelect("NAME", "EMAIL", "UF_CRM_1654007527292");


            //var test = new CrmContact()
            //{

            //    NAME = "loh",
            //};

            //client.Create("crm.contact.add", test).GetAwaiter().GetResult();
            #endregion
        }

        public static List<Contact> ReadContacts()
        {
            using var excel = new ContactWorkSheet(@"C:\Yolva\BitrixSDK\version2.xlsx",1);
            return excel.ReadAllContacts().ToList();
        }
        public static void WriteContacts(IEnumerable<Contact> contacts)
        {
            using var excel = new ContactWorkSheet(@"C:\Yolva\BitrixSDK\version2.xlsx", 2);
            foreach (var contact in contacts)
                excel.WriteContact(contact);
            excel.Save();
        }

        public static async Task Start()
        {
            var listExcel = ReadContacts();
            var count = 0;
            Parallel.ForEach(listExcel, new ParallelOptions { MaxDegreeOfParallelism = 50 }, (le) =>
            {
                if (le.IsMarkedAsGarbadge) return;
                var query = new Bitrix24QueryBuilder("crm.contact.list")
                       .Order("DATE_CREATE", OrderEnum.ASC)
                       .AddFilter("%NAME", le.FirstName)
                       .AddFilter("%LAST_NAME", le.LastName)
                       //.AddFilter("EMAIL", le.WorkEmail)
                       //.AddFilter("%SECOND_NAME", le.MiddleName)
                       .AddSelect("NAME", "LAST_NAME", "SECOND_NAME", "EMAIL", "UF_CRM_1654007527292");
                var client = cachedClients.getClient();
                var list = client.RetrieveListAsync<BitrixResponse<CrmContactAdvanced>>(query).GetAwaiter().GetResult();
                cachedClients.MarkAsFree(client);
                if (list.total == 0) le.IsCreatedInBitrix = true;
                else
                {
                    if (list.result.Any(x => Equal(x.SECOND_NAME, le.MiddleName)) || IsNull(le.MiddleName))
                    {
                        foreach (var b in list.result)
                        {
                            if (b.EMAIL != null && b.EMAIL.Any(y => Equal(y.VALUE, le.PersonalEmail) || Equal(y.VALUE, le.WorkEmail)))
                            {
                                le.IsFullMatch = true;
                            }
                            else
                            {
                                if (b.PHONE != null && b.PHONE.Any(x => x.VALUE == le.WorkPhone || x.VALUE == le.MobilePhone))
                                {
                                    le.IsFullMatch = true;
                                }
                                else
                                {
                                    le.IsPartialMatch = true;
                                }
                            }
                        }
                    }
                    else le.IsPartialMatch = true;
                }
                Interlocked.Increment(ref count);
                Console.WriteLine(count);
            });
            Console.WriteLine("FullMath:" + listExcel.Count(x => x.IsFullMatch));
            Console.WriteLine("PartialMatch:" + listExcel.Count(x => x.IsPartialMatch));
            Console.WriteLine("Created:" + listExcel.Count(x => x.IsCreatedInBitrix));
            WriteContacts(listExcel);
        }
        private static bool Equal(string val1, string val2)
        {
            if (IsNull(val1) && IsNull(val2)) return true;
            return string.Equals(val1?.Trim(), val2?.Trim(), StringComparison.OrdinalIgnoreCase);
        }
        private static bool IsNull(string? str)
            => String.IsNullOrEmpty(str?.Trim());
    }
}