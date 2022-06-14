using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yolva.Bitrix.Extensions
{
    public class QueryList
    {

    }
    public enum OrderEnum
    {
        ASC,
        DESC
    }
    public struct QueryOrder
    {
        public string Key { get; }
        public OrderEnum Value { get; }
        public QueryOrder(string key, OrderEnum value)
        {
            this.Key = key;
            this.Value = value;
        }
    }
    public struct QueryFilter
    {
        public string Key { get; }
        public string Value { get; }
        public QueryFilter(string key, string value)
        {
            this.Key = key;
            this.Value = value;
        }
    }
    public sealed class Bitrix24QueryBuilder
    {
        public readonly string Command;
        private List<QueryFilter> filters;
        private List<string> selectors;
        private QueryOrder? order;
        private int? start=0;
        public Bitrix24QueryBuilder(string command)
        {
            this.Command = command;
        }
        /// <summary>
        /// % - %NAME : "Ал" - частичное совпадение
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Bitrix24QueryBuilder AddFilter(string key, string value)
        {
            filters ??= new();
            if (filters.Any(x => x.Key.ToLower() == key.ToLower()))
                throw new Exception("The same filter key already exist");
            filters.Add(new QueryFilter(key, value));
            return this;
        }
        public Bitrix24QueryBuilder Order(string key, OrderEnum order)
        {
            this.order = new QueryOrder(key, order);
            return this;
        }
        public Bitrix24QueryBuilder Start(int start)
        {
            this.start = start;
            return this;
        }
        /// <summary>
        /// * - все поля
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        public Bitrix24QueryBuilder AddSelect(params string[] selector)
        {
            selectors ??= new();
            selectors.AddRange(selector);
            return this;
        }
        public string Create()
        {
            var sb = new StringBuilder();
            if(start!=null)
            {
                sb.Append("\"start\":" + start + ",");
            }
            if (order != null)
                sb.Append("\"order\": { \""+order.Value.Key+ "\": \"" + order.Value.Value+ "\" },");
            if (filters != null)
            {
                sb.Append("\"filter\": { ");
                var count = filters.Count();
                for (int i = 0; i < count; i++)
                    sb.Append("\"" + filters[i].Key + "\": \"" + filters[i].Value + "\"" + ((i+1) != count ? "," : ""));
                sb.Append(" },");
            }
            if (selectors != null)
            {
                sb.Append("\"select\": [ ");
                var count = selectors.Count();
                for (int i = 0; i < count; i++)
                    sb.Append($"\"{selectors[i]}\"" + ((i+1) != count ? "," : ""));
                sb.Append(" ]");
            }
            return "{ "+sb+" }";
        }
    }
}
