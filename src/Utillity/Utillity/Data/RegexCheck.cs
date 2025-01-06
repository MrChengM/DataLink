using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Utillity.Data
{
    public class RegexCheck
    {
        public static bool IsIPAddress(string source)
        {
            return Regex.IsMatch(source, @"^[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}$");
        }
        public static bool IsNumber(string source)
        {
            return Regex.IsMatch(source, @"^[0-9]*$");
        }
        public static bool IsString(string source)
        {
            return Regex.IsMatch(source, @"^[A-Za-z0-9]+$");
        }
    }
}
