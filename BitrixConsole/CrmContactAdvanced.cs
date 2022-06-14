using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yolva.Bitrix.Client.Entities.Crm;

namespace BitrixConsole
{
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
}
