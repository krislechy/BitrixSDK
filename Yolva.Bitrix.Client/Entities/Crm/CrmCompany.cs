using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yolva.Bitrix.Client.Entities.Crm
{
    public class CrmCompany
    {
        public long ID { get; set; }
        public string COMPANY_TYPE { get; set; }
        public string TITLE { get; set; }
        public LOGO LOGO { get; set; }
        public object LEAD_ID { get; set; }
        public string HAS_PHONE { get; set; }
        public string HAS_EMAIL { get; set; }
        public string HAS_IMOL { get; set; }
        public string ASSIGNED_BY_ID { get; set; }
        public string CREATED_BY_ID { get; set; }
        public string MODIFY_BY_ID { get; set; }
        public object BANKING_DETAILS { get; set; }
        public string INDUSTRY { get; set; }
        public object REVENUE { get; set; }
        public object CURRENCY_ID { get; set; }
        public object EMPLOYEES { get; set; }
        public object COMMENTS { get; set; }
        public DateTime DATE_CREATE { get; set; }
        public DateTime DATE_MODIFY { get; set; }
        public string OPENED { get; set; }
        public string IS_MY_COMPANY { get; set; }
        public object ORIGINATOR_ID { get; set; }
        public object ORIGIN_ID { get; set; }
        public object ORIGIN_VERSION { get; set; }
        public object ADDRESS { get; set; }
        public object ADDRESS_2 { get; set; }
        public object ADDRESS_CITY { get; set; }
        public object ADDRESS_POSTAL_CODE { get; set; }
        public object ADDRESS_REGION { get; set; }
        public object ADDRESS_PROVINCE { get; set; }
        public object ADDRESS_COUNTRY { get; set; }
        public object ADDRESS_COUNTRY_CODE { get; set; }
        public object ADDRESS_LOC_ADDR_ID { get; set; }
        public object ADDRESS_LEGAL { get; set; }
        public object REG_ADDRESS { get; set; }
        public object REG_ADDRESS_2 { get; set; }
        public object REG_ADDRESS_CITY { get; set; }
        public object REG_ADDRESS_POSTAL_CODE { get; set; }
        public object REG_ADDRESS_REGION { get; set; }
        public object REG_ADDRESS_PROVINCE { get; set; }
        public object REG_ADDRESS_COUNTRY { get; set; }
        public object REG_ADDRESS_COUNTRY_CODE { get; set; }
        public object REG_ADDRESS_LOC_ADDR_ID { get; set; }
        public object UTM_SOURCE { get; set; }
        public object UTM_MEDIUM { get; set; }
        public object UTM_CAMPAIGN { get; set; }
        public object UTM_CONTENT { get; set; }
        public object UTM_TERM { get; set; }
        public WEB[] WEB { get; set; }
    }
}
