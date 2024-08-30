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
        public static string[] SplitEndWith(string source, string splitStr = "[", string endStr = "]")
        {
            if (source.EndsWith(endStr))
            {

                var index = source.LastIndexOf(splitStr);
                var startStr = source.Substring(0, index);
                var lastStr = source.Substring(index+1).Replace(endStr,"");
                var arraryString = new string[]{startStr,lastStr};
                //for (int i = 0; i < arraryString.Length; i++)
                //{
                //    arraryString[i] = arraryString[i].Replace(endStr, "");
                //}
                return arraryString;
            }
            else
            {
                return new string[] { source };
            } 
           
        }
       
    }
}
