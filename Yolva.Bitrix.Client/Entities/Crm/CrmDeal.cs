using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yolva.Bitrix.Client.Entities.Crm
{
    public class CrmDeal
    {
        public long ID { get; set; }
        public string TITLE { get; set; }
        public string TYPE_ID { get; set; }
        public string STAGE_ID { get; set; }
        public object PROBABILITY { get; set; }
        public string CURRENCY_ID { get; set; }
        public string OPPORTUNITY { get; set; }
        public string IS_MANUAL_OPPORTUNITY { get; set; }
        public object TAX_VALUE { get; set; }
        public object LEAD_ID { get; set; }
        public string COMPANY_ID { get; set; }
        public long CONTACT_ID { get; set; }
        public object QUOTE_ID { get; set; }
        public DateTime BEGINDATE { get; set; }
        public DateTime CLOSEDATE { get; set; }
        public string ASSIGNED_BY_ID { get; set; }
        public string CREATED_BY_ID { get; set; }
        public string MODIFY_BY_ID { get; set; }
        public DateTime DATE_CREATE { get; set; }
        public DateTime DATE_MODIFY { get; set; }
        public string OPENED { get; set; }
        public string CLOSED { get; set; }
        public string COMMENTS { get; set; }
        public object ADDITIONAL_INFO { get; set; }
        public object LOCATION_ID { get; set; }
        public string CATEGORY_ID { get; set; }
        public string STAGE_SEMANTIC_ID { get; set; }
        public string IS_NEW { get; set; }
        public string IS_RECURRING { get; set; }
        public string IS_RETURN_CUSTOMER { get; set; }
        public string IS_REPEATED_APPROACH { get; set; }
        public string SOURCE_ID { get; set; }
        public string SOURCE_DESCRIPTION { get; set; }
        public string ORIGINATOR_ID { get; set; }
        public string ORIGIN_ID { get; set; }
        public string MOVED_BY_ID { get; set; }
        public DateTime MOVED_TIME { get; set; }
        public object UTM_SOURCE { get; set; }
        public object UTM_MEDIUM { get; set; }
        public object UTM_CAMPAIGN { get; set; }
        public object UTM_CONTENT { get; set; }
        public object UTM_TERM { get; set; }
    }
}
