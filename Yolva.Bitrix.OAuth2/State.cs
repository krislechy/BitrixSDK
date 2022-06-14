using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yolva.Bitrix.OAuth2
{
    internal static class State
    {
        private const string _chars = "abcdefghijklmnopqrstuvwxyz0123456789";
        private static Random random = new Random();
        public static string GenerateState(int length = 17, string chars = _chars) =>
            new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}
