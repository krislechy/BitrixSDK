using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yolva.Bitrix.Client.Entities.Crm
{
    public class CrmLead
    {
        public long ID { get; set; }
        public string TITLE { get; set; }
        public object HONORIFIC { get; set; }
        public string NAME { get; set; }
        public object SECOND_NAME { get; set; }
        public string LAST_NAME { get; set; }
        public object COMPANY_TITLE { get; set; }
        public object COMPANY_ID { get; set; }
        public long CONTACT_ID { get; set; }
        public string IS_RETURN_CUSTOMER { get; set; }
        public string BIRTHDATE { get; set; }
        public string SOURCE_ID { get; set; }
        public string SOURCE_DESCRIPTION { get; set; }
        public string STATUS_ID { get; set; }
        public object STATUS_DESCRIPTION { get; set; }
        public object POST { get; set; }
        public string COMMENTS { get; set; }
        public string CURRENCY_ID { get; set; }
        public string OPPORTUNITY { get; set; }
        public string IS_MANUAL_OPPORTUNITY { get; set; }
        public string HAS_PHONE { get; set; }
        public string HAS_EMAIL { get; set; }
        public string HAS_IMOL { get; set; }
        public string ASSIGNED_BY_ID { get; set; }
        public string CREATED_BY_ID { get; set; }
        public string MODIFY_BY_ID { get; set; }
        public DateTime DATE_CREATE { get; set; }
        public DateTime DATE_MODIFY { get; set; }
        public string DATE_CLOSED { get; set; }
        public string STATUS_SEMANTIC_ID { get; set; }
        public string OPENED { get; set; }
        public object ORIGINATOR_ID { get; set; }
        public object ORIGIN_ID { get; set; }
        public string MOVED_BY_ID { get; set; }
        public DateTime MOVED_TIME { get; set; }
        public object ADDRESS { get; set; }
        public object ADDRESS_2 { get; set; }
        public object ADDRESS_CITY { get; set; }
        public object ADDRESS_POSTAL_CODE { get; set; }
        public object ADDRESS_REGION { get; set; }
        public object ADDRESS_PROVINCE { get; set; }
        public object ADDRESS_COUNTRY { get; set; }
        public object ADDRESS_COUNTRY_CODE { get; set; }
        public object ADDRESS_LOC_ADDR_ID { get; set; }
        public object UTM_SOURCE { get; set; }
        public object UTM_MEDIUM { get; set; }
        public object UTM_CAMPAIGN { get; set; }
        public object UTM_CONTENT { get; set; }
        public object UTM_TERM { get; set; }
        public EMAIL[] EMAIL { get; set; }
        public PHONE[] PHONE { get; set; }
    }
}
