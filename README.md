# BitrixSDK
Create Client:
```cs
public IBitrixService BitrixCreateClient()
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
}
```
Retrieve Entity (example: Crm.Contact):
```cs
var query = new Bitrix24QueryBuilder("crm.contact.list")
                    .AddSelect("ID", "NAME", "LAST_NAME", "SECOND_NAME", "EMAIL", "PHONE", "UF_CRM_1654007527292");
                    .AddFilter("NAME","Alexander") //$NAME, !NAME, >NAME
                    .Order("NAME",OrderEnum.ASC);
var response=await client.RetrieveListAsync<BitrixResponse<CrmContactAdvanced>>(query);
```
Retrieve all records of Entity (example: Crm.Contact):
```cs
var query = new Bitrix24QueryBuilder("crm.contact.list")
  .AddSelect("ID", "NAME", "LAST_NAME", "SECOND_NAME", "EMAIL", "PHONE", "UF_CRM_1654007527292");
var contacts = new List<CrmContactAdvanced>();
BitrixResponse<CrmContactAdvanced> response;
int next = 0;
do
{
  query.Start(next);
  response = await client.RetrieveListAsync<BitrixResponse<CrmContactAdvanced>>(query);
  contacts.AddRange(response?.result);
  next += 50;//max step 50
} while (response.next != null && response.next > 0);
```
Create (example: Crm.Contact):
```cs
client.CreateAsync<CrmContactAdvanced>("crm.contact.add",new CrmContactAdvanced
            {
                NAME ="loh",
                LAST_NAME="pidor"
            })
```
Update (example: Crm.Contact):
```cs
await client.UpdateAsync<CrmContactAdvanced>("crm.contact.update",new CrmContactAdvanced
            {
                ID= 42055,
                NAME ="loh",
                LAST_NAME="pidor"
            })
```
Delete (example: Crm.Contact):
```cs
await client.DeleteAsync("crm.contact.delete",42055);
```
You can inherit several basic entity (Crm.Company,Crm.Contact,Crm.Deal,Crm.Lead) classes (Example: Crm.Contact):
<details>
  <summary>Example</summary>
      
```cs
    public class CrmContactAdvanced : CrmContact
    {
        [JsonProperty("UF_CRM_1651072052487")]
        public long? LevelJobTitle { get; set; }

        [JsonProperty("UF_CRM_1653304186684")]
        public long? AreaOfResponsibility { get; set; }
        [JsonProperty("UF_CRM_1654587148")]
        public string? VersionOfUpload { get; set; }
        [JsonProperty("UF_CRM_1654081507849")]
        public bool? ChangeAfterUpload { get; set; }
        #region Allow

        #region Разрешить рассылку по емейл
        [JsonIgnore]
        public bool AllowMailingList { get; set; } = true;

        [JsonProperty("UF_CRM_1651072592783")]
        public long _AllowMailingList { get => AllowMailingList ? 139 : 140; }
        #endregion
        #region Электронная почта
        [JsonIgnore]
        public bool AllowEmail { get; set; } = true;

        [JsonProperty("UF_CRM_1651072611458")]
        public long _AllowEmail { get => AllowEmail ? 141 : 142; }
        #endregion
        #region Звонки
        [JsonIgnore]
        public bool AllowCall { get; set; } = true;

        [JsonProperty("UF_CRM_1651072629406")]
        public long _AllowCall { get => AllowCall ? 143 : 144; }
        #endregion
        #region Факсы
        [JsonIgnore]
        public bool AllowFax { get; set; } = true;

        [JsonProperty("UF_CRM_1651072651716")]
        public long _AllowFax { get => AllowFax ? 145 : 146; }
        #endregion
        #region Почта
        [JsonIgnore]
        public bool? AllowMail { get; set; } = true;

        [JsonProperty("UF_CRM_1651072675328")]
        public long _AllowMail { get => AllowMail==null?267:(AllowMail.Value ? 147 : 148); }
        #endregion
        #region Участвует в экспорте контактов
        [JsonIgnore]
        public bool AllowExport { get; set; } = true;

        [JsonProperty("EXPORT")]
        public char _AllowExport { get => AllowExport ? 'Y' : 'N'; }
        #endregion
        #region Активный
        [JsonIgnore]
        public bool IsActive { get; set; } = true;

        [JsonProperty("UF_CRM_1651072757837")]
        public long _IsActive { get => IsActive ? 149 : 150; }
        #endregion
        #endregion
    }
```
</details>
