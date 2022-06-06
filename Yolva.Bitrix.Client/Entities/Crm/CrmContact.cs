using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yolva.Bitrix.Client.Entities.Crm
{
    public class CrmContact
    {
        public string ID { get; set; }
        public string POST { get; set; }
        public object COMMENTS { get; set; }
        public object HONORIFIC { get; set; }
        public string NAME { get; set; }
        public string SECOND_NAME { get; set; }
        public string LAST_NAME { get; set; }
        public object PHOTO { get; set; }
        public object LEAD_ID { get; set; }
        public string TYPE_ID { get; set; }
        public IEnumerable<PHONE> PHONE { get; set; }
        public string SOURCE_ID { get; set; }
        public object SOURCE_DESCRIPTION { get; set; }
        public string COMPANY_ID { get; set; }
        public string BIRTHDATE { get; set; }
        public string EXPORT { get; set; }
        public string HAS_PHONE { get; set; }
        public string HAS_EMAIL { get; set; }
        public string HAS_IMOL { get; set; }
        public DateTime DATE_CREATE { get; set; }
        public DateTime DATE_MODIFY { get; set; }
        public string ASSIGNED_BY_ID { get; set; }
        public string CREATED_BY_ID { get; set; }
        public string MODIFY_BY_ID { get; set; }
        public string OPENED { get; set; }
        public object ORIGINATOR_ID { get; set; }
        public object ORIGIN_ID { get; set; }
        public object ORIGIN_VERSION { get; set; }
        public object FACE_ID { get; set; }
        public object ADDRESS { get; set; }
        public object ADDRESS_2 { get; set; }
        public object ADDRESS_CITY { get; set; }
        public object ADDRESS_POSTAL_CODE { get; set; }
        public object ADDRESS_REGION { get; set; }
        public object ADDRESS_PROVINCE { get; set; }
        public object ADDRESS_COUNTRY { get; set; }
        public object ADDRESS_LOC_ADDR_ID { get; set; }
        public object UTM_SOURCE { get; set; }
        public object UTM_MEDIUM { get; set; }
        public object UTM_CAMPAIGN { get; set; }
        public object UTM_CONTENT { get; set; }
        public object UTM_TERM { get; set; }
        public string UF_CRM_1651072052487 { get; set; }
        public string UF_CRM_1651072592783 { get; set; }
        public string UF_CRM_1651072611458 { get; set; }
        public string UF_CRM_1651072629406 { get; set; }
        public string UF_CRM_1651072651716 { get; set; }
        public string UF_CRM_1651072675328 { get; set; }
        public string UF_CRM_1651072757837 { get; set; }
        public string UF_CRM_1651126438417 { get; set; }
        public IEnumerable<object> UF_CRM_1652255529617 { get; set; }
        public string UF_CRM_1653028466683 { get; set; }
        public string UF_CRM_1653028481172 { get; set; }
        public string UF_CRM_1653304186684 { get; set; }
        public string UF_CRM_1654007527292 { get; set; }
        public string UF_CRM_1654081507849 { get; set; }
        public string UF_CRM_629891F140A30 { get; set; }
        public string UF_CRM_629891F19E33F { get; set; }
        public string UF_CRM_629891F203511 { get; set; }
        public string UF_CRM_629891F260CAB { get; set; }
        public string UF_CRM_629891F2BD37C { get; set; }
        public IEnumerable<EMAIL> EMAIL { get; set; }
    }
}
