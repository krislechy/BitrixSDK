using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yolva.Bitrix.Client
{
    internal struct RequestContentBitrix<T>
    {
        public long? id { get; set; }
        public T fields { get; set; }
    }
}
