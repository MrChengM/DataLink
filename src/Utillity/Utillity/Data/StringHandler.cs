using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utillity.Data
{
    public static class StringHandler
    {
        public static string[] Split(string source, string splitStr = "[", string removeStr = "]")
        {
            var arraryString = source.Split(splitStr.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < arraryString.Length; i++)
            {
                arraryString[i] = arraryString[i].Replace(removeStr, "");
            }
            return arraryString;
        }
    }
}
