using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daktbot.Common.Utilities
{
    public static class IdentityHelper
    {
        private static Random _random = new Random();
        private const int ID_SIZE = 12;

        private static char[] IdentityAcceptableChars =
        {
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'w', 'z',
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'W', 'Z',
            '1', '2', '3', '4', '5', '6', '7', '8', '9', '0'
        };

        public static string GenerateIdentity(string idPrefix) 
        {
            StringBuilder newIdString = new StringBuilder($"{idPrefix}_");
            for (int i = 0; i < ID_SIZE; i++)
            {
                newIdString.Append(IdentityAcceptableChars[_random.Next(0, IdentityAcceptableChars.Length - 1)]);
            }

            return newIdString.ToString();
        }

        public static bool IsMatch(string id1, string id2) 
        {
            return string.Equals(id1, id2, StringComparison.Ordinal);
        }
    }
}
