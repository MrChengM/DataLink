using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utillity.Security;

namespace UtillityTest
{
  public class Program
    {
        static void Main(string[] args)
        {
            string str = "GMGMGMGMGMGMGMGM";
            var salt=SHA256Code.ConvertToSalt(str);
            string psd = "1@Password";
            var result= SHA256Code.HashPasswordWithSalt(psd, salt);
            Console.WriteLine(result);
            Console.ReadLine();
        }
    }
}
