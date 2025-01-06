
using DataServer.Config;
using WCFRestFullAPI.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Utillity.File;
using System.Runtime.Serialization;

namespace RestAPIServerEx
{
    class Program
    {
        ////private const string CONFIGPATH = "../../../../conf";
        ////private const string PROJECTFILE_DEFAULT = "/ProjectConfig.Josn";
        ////private const string DLLPATH = "../../../../dll";

        //private const string restServerAddress = "http://127.0.0.1:8008/ConfigService";
        static void Main(string[] args)
        {

            //var config = JsonFunction.Load<ProjectConfig>(CONFIGPATH + PROJECTFILE_DEFAULT);
            ConfigRestServerHost configRestServerHost = new ConfigRestServerHost();
            configRestServerHost.Open();
            Console.ReadLine();
        }
    }
}
