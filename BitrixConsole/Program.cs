using BitrixConsole.Models.Excel;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using Yolva.Bitrix.Client;
using Yolva.Bitrix.Client.Abstractions;
using Yolva.Bitrix.Client.Entities;
using Yolva.Bitrix.Client.Entities.Crm;
using Yolva.Bitrix.Extensions;
using Yolva.Bitrix.OAuth2;

namespace BitrixConsole
{
    #region CachedClient
    public sealed class CachedClients<T> where T : class
    {
        public class CC<T>
        {
            public T client { get; set; }
            public int countUsed { get; set; }=1;
            public DateTime created { get; set; } = DateTime.Now;
        }
        List<CC<T>> lazy;
        Func<T> funcNewClient;
        public CachedClients(Func<T> func)
        {
            lazy = new();
            funcNewClient = func;
        }
        object sync=new object();
        object sync2=new object();
        object sync3=new object();
        private T createClient()
        {
            lock (sync2)
            {
                var cl = funcNewClient();
                lazy.Add(new CC<T> { client = cl });
                return cl;
            }
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
            lock (sync3)
            {
                foreach (var s in lazy)
                {
                    if (s.client == client)
                    {
                        s.countUsed--;
                        return;
                    }
                }
            }
        }
    }
    #endregion
    public class Program
    {
        private static IConfigurationRoot configuration;
        private static CachedClients<IBitrixService> cachedClients;
        private static ComplianceNames complianceNames;

        static Program()
        {
            complianceNames = new ComplianceNames("ComplianceNames.json");
            configuration = new ConfigurationBuilder()
                        .AddJsonFile("appsettings.json")
                        .Build();
            cachedClients = new CachedClients<IBitrixService>(()=>BitrixCreateClient());
        }

        public static void Main(string[] args)
        {
            //OldAlgh().GetAwaiter().GetResult();//1
            //Upload();
            //RemoveAllWithoutConnected().GetAwaiter().GetResult();
        }
        #region retrievedAll
        public static async Task<List<CrmContactAdvanced>> RetrievedAllContacts()
        {
            var contacts = new List<CrmContactAdvanced>();
            if (File.Exists("contacts.json"))
                contacts.AddRange(JsonConvert.DeserializeObject<List<CrmContactAdvanced>>(File.ReadAllText("contacts.json")));
            else
            {
                var query = new Bitrix24QueryBuilder("crm.contact.list")
                    .AddSelect("ID", "NAME", "LAST_NAME", "SECOND_NAME", "EMAIL", "PHONE", "UF_CRM_1654007527292");
                var client = cachedClients.getClient();
                BitrixResponse<CrmContactAdvanced> response;
                int next = 0;
                do
                {
                    query.Start(next);
                    response = await client.RetrieveListAsync<BitrixResponse<CrmContactAdvanced>>(query);
                    contacts.AddRange(response?.result);
                    next += 50;
                    Console.WriteLine(next);
                } while (response.next != null && response.next > 0);
                var strJ = JsonConvert.SerializeObject(contacts);
                File.WriteAllText("contacts.json", strJ);
            }
            return contacts;
        }
        public static async Task<List<CrmLead>> RetrievedAllLeads()
        {
            var leads = new List<CrmLead>();
            if (File.Exists("leads.json"))
                leads.AddRange(JsonConvert.DeserializeObject<List<CrmLead>>(File.ReadAllText("leads.json")));
            else
            {
                var query = new Bitrix24QueryBuilder("crm.lead.list")
                    .AddSelect("CONTACT_ID")
                    .AddFilter("!CONTACT_ID","null");
                var client = cachedClients.getClient();
                BitrixResponse<CrmLead> response;
                int next = 0;
                do
                {
                    query.Start(next);
                    response = await client.RetrieveListAsync<BitrixResponse<CrmLead>>(query);
                    leads.AddRange(response?.result);
                    next += 50;
                    Console.WriteLine(next);
                } while (response.next != null && response.next > 0);
                var strJ = JsonConvert.SerializeObject(leads);
                File.WriteAllText("leads.json", strJ);
            }
            return leads;
        }
        public static async Task<List<CrmDeal>> RetrievedAllDeals()
        {
            var deals = new List<CrmDeal>();
            if (File.Exists("deals.json"))
                deals.AddRange(JsonConvert.DeserializeObject<List<CrmDeal>>(File.ReadAllText("deals.json")));
            else
            {
                var query = new Bitrix24QueryBuilder("crm.deal.list")
                     .AddSelect("CONTACT_ID")
                    .AddFilter("!CONTACT_ID", "null");
                var client = cachedClients.getClient();
                BitrixResponse<CrmDeal> response;
                int next = 0;
                do
                {
                    query.Start(next);
                    response = await client.RetrieveListAsync<BitrixResponse<CrmDeal>>(query);
                    deals.AddRange(response?.result);
                    next += 50;
                    Console.WriteLine(next);
                } while (response.next != null && response.next > 0);
                var strJ = JsonConvert.SerializeObject(deals);
                File.WriteAllText("deals.json", strJ);
            }
            return deals;
        }
        #endregion
        #region another
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
        public static List<Contact> ReadContactsFromExcel(int workbook=1)
        {
            using var excel = new ContactWorkSheet(@"C:\Yolva\BitrixSDK\version2.xlsx",workbook);
            return excel.ReadAllContacts().ToList();
        }
        public static void WriteContactsToExcel(IEnumerable<Contact> contacts, int workbook = 2)
        {
            using var excel = new ContactWorkSheet(@"C:\Yolva\BitrixSDK\version2.xlsx", workbook);
            foreach (var contact in contacts)
                excel.WriteContact(contact);
            excel.Save();
        }
        #endregion

        private static List<CrmContactAdvanced> GetConnectedContacts()
        {
            List<CrmContactAdvanced> dict = new List<CrmContactAdvanced>();
            if (File.Exists("connectedContacts.json"))
                dict.AddRange(JsonConvert.DeserializeObject<List<CrmContactAdvanced>>(File.ReadAllText("connectedContacts.json")));
            else
            {
                var client = cachedClients.getClient();
                var deals = RetrievedAllDeals().Result.DistinctBy(x => x.CONTACT_ID).Select(x => x.CONTACT_ID);
                var leads = RetrievedAllLeads().Result.DistinctBy(x => x.CONTACT_ID).Select(x => x.CONTACT_ID);
                var list = new List<long>();
                list.AddRange(deals);
                list.AddRange(leads);
                list = list.Distinct().ToList();
                
                foreach (var s in list)
                {
                    var e = new Bitrix24QueryBuilder("crm.contact.list")
                         .AddSelect("ID", "NAME", "LAST_NAME", "SECOND_NAME", "EMAIL", "PHONE")
                        .AddFilter("ID", s.ToString());
                    var emails = client.RetrieveListAsync<BitrixResponse<CrmContactAdvanced>>(e).Result;
                    
                    dict.AddRange(emails.result);
                }
                var json = JsonConvert.SerializeObject(dict);
                File.WriteAllText("connectedContacts.json", json);
            }
            return dict;
        }
        private static async Task RemoveAllWithoutConnected()
        {
            var listBitrix = await RetrievedAllContacts();
            var removed=listBitrix.RemoveAll(x => x.EMAIL==null);
            Console.WriteLine("Removed from list all with empty EMAIL: "+removed);
            var listconnected = GetConnectedContacts();
            Console.WriteLine($"Count In Bitrix: {listBitrix.Count()}");
            List<CrmContactAdvanced> listToDelete = new List<CrmContactAdvanced>();
            foreach (var contact in listBitrix)
            {
                if (!listconnected.Any(x => x.ID == contact.ID))
                {
                    listToDelete.Add(contact);
                }
            }
            Console.WriteLine($"Count to Delete: {listToDelete.Count()}");
            var countDeleted = 0;
            var totaltoDelete = listToDelete.Count();
            var client = cachedClients.getClient();
            Parallel.ForEach(listToDelete, new ParallelOptions() { MaxDegreeOfParallelism = 15 }, (s) =>
            //foreach (var s in listToDelete)
            {
                try
                {
                    client.DeleteAsync("crm.contact.delete", s.ID).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                Interlocked.Increment(ref countDeleted);
                Console.WriteLine($"{countDeleted}/{totaltoDelete}");
            });
        }
        #region Upload
        private static void Upload()
        {
            //var e=cachedClients.getClient();
            var contacts=ReadContactsFromExcel(2);
            int countCreated = 0;
            foreach(var s in contacts)
            {
                if (s.IdInBitrix == null && s.IsMarkedAsGarbadge == false && s.IsInvalidEmail==false && s.RowId > 9716)
                {
                    Console.WriteLine($"===================================");
                    Console.WriteLine($"Looking for: {s.RowId}");
                    var contactId = CreateContact(s);
                    Console.WriteLine($"Created: {contactId}");
                    var companyId = FindCompanyOrCreate(s);
                    Console.WriteLine($"Company ID: {(companyId == null ? "-Without Company-" : companyId)}");
                    if (companyId != null)
                    {
                        var isConnected = AddContactToCompany(contactId, companyId.Value);
                        if (isConnected)
                            Console.WriteLine($"Successfully connected: {contactId}->{companyId}");
                        else throw new Exception("fdfg");
                    }
                    Console.WriteLine($"===================================");
                    countCreated++;
                    Console.WriteLine("res: " + countCreated);
                    //if (countCreated % 1000 == 0)
                    //    Console.ReadKey();
                }
                //if (s.IdInBitrix != null && s.IsMarkedAsGarbadge == false)
                //{
                //    Console.WriteLine(s.IdInBitrix);
                //    ClearPhonesBtx(long.Parse(s.IdInBitrix));
                //    ClearEmailsBtx(long.Parse(s.IdInBitrix));
                //    Update(s, long.Parse(s.IdInBitrix));
                //    countUpdated++;
                //    //if (countUpdated % 5 == 0)
                //    //{
                //    //    Console.WriteLine("5 updated");
                //    //    Console.ReadKey();
                //    //}
                //    Console.WriteLine("res: "+countUpdated);
                //}
            }
        }
        private static void Update(Contact contact,long idBitrix)
        {
            var c = Convert(contact);
            c.ID = idBitrix;

            var client = cachedClients.getClient();
            client.UpdateAsync<CrmContactAdvanced>("crm.contact.update", c).GetAwaiter().GetResult();
            cachedClients.MarkAsFree(client);
        }
        private static long CreateContact(Contact contact)
        {
            var c = Convert(contact);
            c.VersionOfUpload = DateTime.Now.ToString("dd.MM.yyyy");
            var client=cachedClients.getClient();
            var contactId=client.CreateAsync<CrmContactAdvanced, long>("crm.contact.add", c).Result;
            cachedClients.MarkAsFree(client);
            return contactId;
        }
        private static CrmContactAdvanced Convert(Contact contact) =>
            new CrmContactAdvanced()
            {
                LAST_NAME = contact.LastName??"",
                NAME = contact.FirstName??"",
                SECOND_NAME = contact.MiddleName??"",
                EMAIL = CreateEmailBtx(contact),
                PHONE = CreatePhoneBtx(contact),
                POST = contact.JobTitle??"",
                LevelJobTitle = GetLevelJobTitle(contact.LevelJobTitle),
                AreaOfResponsibility = GetAreaOfResponsibility(contact.AreaOfResponsibility),
                AllowMailingList= !IsExistInDenyEmails(contact.WorkEmail),
                AllowEmail= true,
                AllowCall=true,
                AllowFax=true,
                AllowMail=true,
                AllowExport=true,
                IsActive=true,
            };
        private static long? GetLevelJobTitle(string? name)
        {
            switch (name)
            {
                case ("Владелец бизнеса"): return 135;
                case ("Топ менеджмент"):return 136;
                case ("Линейный руководитель"):return 137;
                case ("Специалист"):return 138;
                case ("Менеджер"):return 292;
                case ("Сотрудник"):return 293;
                default:return null;
            }
        }
        private static long? GetAreaOfResponsibility(string? name)
        {
            switch (name)
            {
                case ("ИТ"): return 278;
                case ("Логистика"): return 280;
                case ("Маркетинг"): return 277;
                case ("Общее управление"): return 276;
                case ("Операции"): return 281;
                case ("Персонал"): return 282;
                case ("Продажи"): return 279;
                case ("Финансы"): return 283;
                default: return null;
            }
        }
        private static bool AddContactToCompany(long contactId, long companyId)
        {
            var client = cachedClients.getClient();
            var res= client.CreateAsync<object, bool>("crm.company.contact.add", new
            {
                CONTACT_ID = contactId,
            }, companyId).Result;
            cachedClients.MarkAsFree(client);
            return res;
        }
        private static long? FindCompanyOrCreate(Contact contact)
        { 
            if(contact.CompanyName==null) return null;
            Console.WriteLine($"Search company: {contact.CompanyName}");
            var client = cachedClients.getClient();
            var query = new Bitrix24QueryBuilder("crm.company.list");
            query.AddFilter("TITLE", contact.CompanyName
                .Replace("\"","")
                .Replace("\\","")
                )
                .AddSelect("ID", "TITLE");
            var companies = client.RetrieveListAsync<BitrixResponse<CrmCompany>>(query).Result;
            var resultCount = companies.result.Count();
            if (resultCount == 0)
            {
                var idCreated=client.CreateAsync<CrmCompany, long>("crm.company.add", new CrmCompany()
                {
                    TITLE = contact.CompanyName.Trim()
                }).Result;
                cachedClients.MarkAsFree(client);
                Console.WriteLine($"CREATED company: {idCreated}");
                return idCreated;
            }
            if (resultCount > 1)
            {
                foreach (var s in companies.result)
                    Console.WriteLine($"{s.ID} : {s.TITLE}");
                Console.WriteLine($"---------------");
                Console.WriteLine($"0 : SKIP");
                Console.WriteLine($"1 : CREATE NEW");
                var isidhand = long.TryParse(Console.ReadLine(), out var handid);
                if (isidhand)
                {
                    if (companies.result.Any(x => x.ID == handid))
                    {
                        return handid;
                    }
                    else
                    {
                        if (handid == 0)
                            return null;
                        else if(handid==1)
                        {
                            var res = client.CreateAsync<CrmCompany, long>("crm.company.add", new CrmCompany()
                            {
                                TITLE = contact.CompanyName.Trim()
                            }).Result;
                            cachedClients.MarkAsFree(client);
                            return res;
                        }
                        else return FindCompanyOrCreate(contact);
                    }
                }
                else
                {
                    return FindCompanyOrCreate(contact);
                }
            }
            cachedClients.MarkAsFree(client);
            var firstCompany = companies.result.First().ID;
            Console.WriteLine($"Found company: {firstCompany}");
            return firstCompany;
        }
        private static List<EMAIL> CreateEmailBtx(Contact contact)
        {
            if(contact.WorkEmail!=null && !contact.IsInvalidEmail)
            {
                var splitted = contact.WorkEmail.Split(new char[] {',',';'});
                var list = new List<EMAIL>();
                foreach (var s in splitted)
                {
                    list.Add(new EMAIL()
                    {
                        VALUE=s,
                    });
                }
                return list;
            }
            return null!;
        }
        private static List<PHONE> CreatePhoneBtx(Contact contact)
        {
            var list = new List<PHONE>();
            if (contact.MobilePhone != null)
                foreach (var s in toCommonFormatPhone(contact.MobilePhone))
                {
                    list.Add(new PHONE()
                    {
                        VALUE = s
                    });
                }
            if (contact.WorkPhone != null)
                foreach (var s in toCommonFormatPhone(contact.WorkPhone))
                {
                    list.Add(new PHONE()
                    {
                        VALUE = s
                    });
                }
            return list;
        }
        private static void ClearPhonesBtx(long idBitrix)
        {
            var client = cachedClients.getClient();
            var query = new Bitrix24QueryBuilder("crm.contact.list")
                .AddFilter("ID", idBitrix.ToString())
                .AddSelect("PHONE");
            var result = client.RetrieveListAsync<BitrixResponse<CrmContactAdvanced>>(query).Result;
            if (result.result.First().PHONE == null) return;
            result.result.First().PHONE.ToList().ForEach(x => x.VALUE = "");

            cachedClients.getClient()
                .UpdateAsync("crm.contact.update", new
                {
                    ID = idBitrix,
                    PHONE = result.result.First().PHONE,
                }).GetAwaiter().GetResult();
            cachedClients.MarkAsFree(client);
        }
        private static void ClearEmailsBtx(long idBitrix)
        {
            var client = cachedClients.getClient();
            var query = new Bitrix24QueryBuilder("crm.contact.list")
                .AddFilter("ID", idBitrix.ToString())
                .AddSelect("EMAIL");
            var result = client.RetrieveListAsync<BitrixResponse<CrmContactAdvanced>>(query).Result;
            if (result.result.First().EMAIL == null) return;
            result.result.First().EMAIL.ToList().ForEach(x => x.VALUE = "");

            cachedClients.getClient()
                .UpdateAsync("crm.contact.update", new
                {
                    ID = idBitrix,
                    EMAIL = result.result.First().EMAIL,
                }).GetAwaiter().GetResult();
            cachedClients.MarkAsFree(client);
        }
        private static List<string> denyMailingList;
        private static List<string> GetDenyMailingList()
        {
            if (denyMailingList == null)
            {
                var emailstext = File.ReadAllText("denyemail.txt");
                var emails = emailstext.Split("\r\n");
                denyMailingList = new();
                denyMailingList.AddRange(emails);
            }
            return denyMailingList;
        }
        private static bool IsExistInDenyEmails(string email)
        {
            if (IsNull(email)) return false;
            GetDenyMailingList();
            var isexist=denyMailingList.Any(x => Equal(email, x));
            Console.WriteLine($"Is Exist in Deny Mailing list: {isexist}");
            return isexist;
        }
        #endregion
        private static void DistinctContact(ref List<Contact> list)
        {
            var nList = new List<Contact>();
            foreach (var s in list)
            {
                if (s.PersonalEmail == null)
                    nList.Add(s);
                else
                {
                    if (!nList.Any(x => Equal(x.PersonalEmail, s.PersonalEmail)))
                        nList.Add(s);
                    else Console.WriteLine("duplicate:" + s.PersonalEmail);
                }
            }
            var fList = new List<Contact>();
            foreach (var s in nList)
            {
                if (s.WorkEmail == null)
                    fList.Add(s);
                else
                {
                    if (!fList.Any(x => Equal(x.WorkEmail, s.WorkEmail)))
                        fList.Add(s);
                    else Console.WriteLine("duplicate:" + s.WorkEmail);
                }
            }
        }
        private static void WriteAllDuplicatePhones(List<Contact> list)
        {
            var groupped = list
                .Where(x => x.MobilePhone != null)
                .GroupBy(x => x.MobilePhone)
                .Where(x => 
                    x.Count() > 1 && 
                    x.GroupBy(x=>x.CompanyName,StringComparer.OrdinalIgnoreCase).Count()>1
                );
            var sb = "";
            foreach (var s in groupped)
            {
                foreach(var phone in s)
                {
                    sb += ($"=>{phone.RowId}");
                }
                sb += "\n";
            }
            File.WriteAllText("duplPhone.txt", sb);
            Console.WriteLine(groupped.Count());
        }
        public static async Task OldAlgh()
        {
            var listExcel = ReadContactsFromExcel();
            //listExcel.RemoveAll(x => IsNull(x.PersonalEmail) && IsNull(x.WorkEmail) && IsNull(x.MobilePhone) && IsNull(x.WorkPhone));
            var listconnected=GetConnectedContacts();//1
            DistinctContact(ref listExcel);
            WriteAllDuplicatePhones(listExcel);

            var listBitrix = await RetrievedAllContacts();
            listBitrix.RemoveAll(x => x.EMAIL == null && x.PHONE == null);

            var totalCount = 0;
            var didNothing = 0;
            var listFullMatch = new List<CrmContactAdvanced>();
            var duplicateInBitrix = new List<CrmContactAdvanced[]>();
            foreach (var row in listExcel)
            {
                //if (row.RowId == 8546)
                //    Console.WriteLine("dfgd");
                //else continue;

                CorrectEmail(row);
                if (row.WorkEmail != null)
                {
                    foreach (var s in listconnected)
                    {
                        if (s.EMAIL != null)
                        {
                            foreach (var email in s.EMAIL)
                            {
                                if (row.WorkEmail.ToLower().Contains(email.VALUE.ToLower()) && PartialMatchContactsName(s,row))
                                {
                                    row.IdInBitrix = s.ID.ToString();
                                }
                            }
                        }
                    }
                }
                if (row.IsMarkedAsGarbadge|| row.IsEmailMatch || row.IsFIOPhoneMatch) continue;
                var matchEmails = listBitrix.Where(x => x.EMAIL != null && x.EMAIL.Any(y => Equal(y.VALUE, row.PersonalEmail) || Equal(y.VALUE, row.WorkEmail)));
                var matchEmailsCount = matchEmails.Count();
                if (matchEmailsCount == 1)
                {
                    row.IsEmailMatch = true;
                    listFullMatch.Add(matchEmails.First());
                }
                else if (matchEmailsCount > 1)
                {
                    duplicateInBitrix.Add(matchEmails.ToArray());
                    listFullMatch.Add(matchEmails.First());
                    row.IsEmailMatch = true;
                }
                else
                {
                    var overlaps = listBitrix.Where(x => PartialMatchContactsName(x, row));
                    var countOverLaps = overlaps.Count();
                    if (countOverLaps == 0)
                    {
                        //row.IsCreatedInBitrix = true;
                        Console.WriteLine("IsCreatedInBitrix");
                    }
                    else
                    {
                        foreach (var btx in overlaps)
                        {
                            if (btx.PHONE != null)
                            {
                                foreach (var phone in toCommonFormatPhone(row.MobilePhone))
                                {
                                    if (btx.PHONE != null && btx.PHONE.Any(x => x.VALUE == phone))
                                    {
                                        if (btx.EMAIL == null)
                                        {
                                            row.IsFIOPhoneMatch = true;
                                            listFullMatch.Add(btx);
                                            break;
                                        }
                                        else
                                            Console.WriteLine(btx.ID);
                                    }
                                }
                                foreach (var phone in toCommonFormatPhone(row.WorkPhone))
                                {
                                    if (btx.PHONE != null && btx.PHONE.Any(x => x.VALUE == phone))
                                    {
                                        if (btx.EMAIL == null)
                                        {
                                            row.IsFIOPhoneMatch = true;
                                            listFullMatch.Add(btx);
                                            break;
                                        }
                                        else
                                            Console.WriteLine(btx.ID);
                                    }
                                }
                                if (!row.IsFIOPhoneMatch)
                                {
                                    //row.IsCreatedInBitrix = true;
                                    Console.WriteLine("IsCreatedInBitrix");
                                    break;
                                }
                                else break;
                            }
                            didNothing++;
                        }
                    }
                }
                Interlocked.Increment(ref totalCount);
                Console.WriteLine(totalCount);
            }
            var notMatchInBitrix = listBitrix.Where(x => !listFullMatch.Any(y => y == x));
            Console.WriteLine("TotalCount in Bitrix: " + listBitrix.Count());
            Console.WriteLine("NotMatchInBitrix: " + notMatchInBitrix.Count());
            Console.WriteLine("DuplicateInBitrix: " + duplicateInBitrix.Count() + "\n");

            Console.WriteLine(String.Join("\n", duplicateInBitrix.Select(x=>$"{string.Join("=",x.Select(c=>c.ID))}")));

            Console.WriteLine("TotalCount in Excel: " + listExcel.Count());
            Console.WriteLine("FIOPhoneMatches: " + listExcel.Count(x => x.IsFIOPhoneMatch));
            Console.WriteLine("EmailMatches: " + listExcel.Count(x => x.IsEmailMatch));
            Console.WriteLine("Skipped: " + didNothing);

            notMatchInBitrix.ToList().ForEach(x =>
            {
                Console.WriteLine("#####");
                Console.Write($"[{x.LAST_NAME}] ");
                Console.Write($"[{x.NAME}] ");
                Console.Write($"[{x.SECOND_NAME}] \n");
                x.EMAIL?.ToList().ForEach(y => Console.WriteLine($"     =>[{y?.VALUE}]"));
                Console.WriteLine("#####");
            });
            ReNumerate(ref listExcel);
            WriteContactsToExcel(listExcel);
        }
        private static void ReNumerate(ref List<Contact> list,int startRow=2)
        {
            int count = startRow;
            foreach(var s in list)
            {
                s.RowId = count;
                count++;
            }
        }
        private static void CorrectEmail(Contact contact)
        {
            if (contact.WorkEmail == null && contact.PersonalEmail == null) return;
            if (contact.PersonalEmail != null)
            {
                contact.PersonalEmail = contact.PersonalEmail.RemoveWhitespace().Trim().Replace("\n", "");
                if (!Equal(contact.WorkEmail, contact.PersonalEmail))
                {
                    if (contact.WorkEmail != null)
                        contact.WorkEmail += ";" + contact.PersonalEmail;
                    else contact.WorkEmail = contact.PersonalEmail;
                }
                contact.PersonalEmail = null;
            }
            if (contact.WorkEmail == null) return;
            contact.WorkEmail = contact.WorkEmail.RemoveWhitespace().Trim().Replace("\n", ""); ;
            if (contact.WorkEmail.EndsWith(","))
                contact.WorkEmail = contact.WorkEmail.Substring(0, contact.WorkEmail.Length - 1);
            var splitted = contact.WorkEmail.Split(";");
            var isInvalidEmail = false;
            foreach (var s in splitted)
                if (!IsValidEmail(s) || !IsValidEmail(s))
                    isInvalidEmail = true;
            contact.IsInvalidEmail = isInvalidEmail;
        }
        private static bool IsValidEmail(string? email)
        {
            if (email == null) return true;
            bool isValid = false;
            try
            {
                MailAddress address = new MailAddress(email);
                isValid = (address?.Address == email);
            }
            catch (FormatException) { }
            return isValid;
        }
        #region eee
        private static bool Equal(string val1, string val2)
        {
            if (IsNull(val1) && IsNull(val2)) return true;
            return string.Equals(val1?.Trim(), val2?.Trim(), StringComparison.OrdinalIgnoreCase);
        }
        private static bool PartialMatchContactsName(CrmContactAdvanced btx, Contact excl)
        {
            var bitrixNames = $"{btx.LAST_NAME ?? ""} {btx.NAME ?? ""} {btx.SECOND_NAME ?? ""}";
            var transBitrix = NickBuhro.Translit.Transliteration.LatinToCyrillic(bitrixNames);
            var compBitrix = "";
            {
                var splitted = bitrixNames.Split(" ").Where(x => !String.IsNullOrWhiteSpace(x));
                foreach (var s in splitted)
                {
                    compBitrix += (compBitrix == "" ? "" : " ") + (complianceNames[s] == null ? NickBuhro.Translit.Transliteration.LatinToCyrillic(s) : s);
                }
            }
            var excelNames = new string[] { excl.LastName ?? "", excl.FirstName ?? "", excl.MiddleName ?? "" };
            int count = 0;
            foreach (var s in excelNames)
            {
                if (string.IsNullOrWhiteSpace(s)) continue;
                if (bitrixNames.Contains(s) || transBitrix.Contains(s) || compBitrix.Contains(s))
                    count++;
            }
            return count > 1;
        }
        private static IEnumerable<string> toCommonFormatPhone(string un_phone)
        {
            if (!String.IsNullOrEmpty(un_phone?.Trim()))
            {
                un_phone = Regex.Replace(un_phone, "[а-яА-Яa-zA-Z]", " ");
                var splitted = un_phone
                    .ToLower()
                    .Split(new char[] { '#', ';', ',','.',' ','/' });
                foreach (var s in splitted)
                {
                    if (!String.IsNullOrEmpty(s?.Trim()))
                    {
                        var phone = s;
                        correctPhone(ref phone);
                        if (phone.Length > 11 && (phone.StartsWith("495")||phone.StartsWith("8")))
                        {
                            var v1 = phone.Substring(0, 11);
                            correctPhone(ref v1);
                            yield return v1;
                            var v2 = phone.Substring(11, phone.Length - 11);
                            correctPhone(ref v2);
                            yield return v2;
                        }
                        yield return phone;
                    }
                }
            }
        }
        private static void correctPhone(ref string phone)
        {
            phone = phone
                .RemoveWhitespace()
                .Replace(".", "")
                .Replace(")", "")
                .Replace("(", "");
            if (phone.StartsWith("+"))
            {
                var iscode = int.TryParse(phone[1].ToString(), out var code);
                if (iscode)
                {
                    var array = phone.ToCharArray();
                    array[1] = (char)(code++);
                    phone = String.Join(String.Empty, array);
                }
                else throw new Exception("loh");
            }
            else if (phone.StartsWith("9") && phone.Length == 10)
            {
                phone = "8" + phone;
            }
            else if (phone.Length == 11 && phone.StartsWith("7"))
                phone = "8" + phone.Substring(1, phone.Length - 1);
            else if (phone.StartsWith("495"))
                phone = "8495" + phone.Substring(3, phone.Length - 3);
        }
        private static bool IsNull(string? str)
            => String.IsNullOrEmpty(str?.Trim());
        #endregion
    }
}