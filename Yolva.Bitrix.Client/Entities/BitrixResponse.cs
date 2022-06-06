using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yolva.Bitrix.Client.Entities
{
    public class BitrixResponse<T> where T : class
    {
        public IEnumerable<T> result { get; set; }
        public int total { get; set; }
        public Time time { get; set; }
    }
}
