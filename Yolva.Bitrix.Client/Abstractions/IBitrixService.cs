using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yolva.Bitrix.Extensions;

namespace Yolva.Bitrix.Client.Abstractions
{
    public interface IBitrixService
    {
        /// <summary>
        /// crm.contact.list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<T?> RetrieveListAsync<T>(Bitrix24QueryBuilder query);
        /// <summary>
        /// Example: crm.contact.add
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<long> Create<T>(string command, T entity);
        /// <summary>
        /// crm.contact.update
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task Update<T>(string command, T? entity);
        /// <summary>
        /// crm.contact.delete
        /// </summary>
        /// <param name="command"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        Task Delete(string command, long id);
    }
}
