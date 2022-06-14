using System.ComponentModel;

namespace Yolva.Bitrix.Extensions
{
    public static class Extensions
    {
        public static string ToParams(this Dictionary<string, string>? dict)
        {
            if (dict != null && dict.Count() > 0)
                return String.Join("&", dict.Select(x => $"{x.Key}={x.Value}"));
            return String.Empty;
        }
        public static DateTime TimeStampToDateTime(this int? timestamp)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            if (timestamp == null) return dateTime;
            dateTime = dateTime.AddSeconds(timestamp.Value).ToLocalTime();
            return dateTime;
        }
        public static string RemoveWhitespace(this string input)
        {
            return new string(input.ToCharArray()
                .Where(c => !Char.IsWhiteSpace(c))
                .ToArray());
        }
        public static T Convert<T>(this string input)
        {
            try
            {
                var converter = TypeDescriptor.GetConverter(typeof(T));
                if (converter != null)
                {
                    // Cast ConvertFromString(string text) : object to (T)
                    return (T)converter.ConvertFromString(input);
                }
                return default(T);
            }
            catch (NotSupportedException)
            {
                return default(T);
            }
        }
    }
}