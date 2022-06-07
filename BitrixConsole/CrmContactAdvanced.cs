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
        [JsonProperty("UF_CRM_1654007527292")]
        public DateTime? VersionUpload { get; set; }
    }
}
