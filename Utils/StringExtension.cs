using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buzz.TxLeague.Women.Config.Utils
{
    public static class StringExtension
    {
        public static string Filter(this string str, List<string> substringToRemove)
        {
            foreach (var c in substringToRemove)
            {
                str = str.Replace(c.ToString(), String.Empty);
            }
            str = string.Join(" ", str.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
            return str;
        }
    }
}
