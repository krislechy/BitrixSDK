using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yolva.Bitrix.Client.Entities
{
    public sealed class Time
    {
        public float start { get; set; }
        public float finish { get; set; }
        public float duration { get; set; }
        public float processing { get; set; }
        public DateTime date_start { get; set; }
        public DateTime date_finish { get; set; }
    }
}
