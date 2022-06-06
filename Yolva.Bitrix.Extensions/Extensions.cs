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
    }
}