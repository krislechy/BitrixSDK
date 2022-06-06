using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yolva.Bitrix.Extensions;

namespace Yolva.Bitrix.Client
{
    public interface IBitrixService
    {
        Task<T?> RetrieveListAsync<T>(Bitrix24QueryBuilder query);
    }
}
