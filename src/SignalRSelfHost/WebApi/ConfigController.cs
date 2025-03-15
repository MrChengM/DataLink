using DataServer;
using DataServer.Config;
using DataServer.Permission;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Utillity.File;
using Utillity.Reflection;

namespace SignalRSelfHost.WebApi
{
    public class ConfigController : ApiController
    {
        private IConfigServer _configServer;

        public ConfigController(IConfigServer configServer)
        {
            _configServer = configServer;
        }
        [ActionName("Project")]
        [HttpGet]
        public ProjectConfig GetProject()
        {
            return  _configServer.Config;
        }
        [ActionName("DriverInformation")]
        [HttpGet]
        public Dictionary<string, DriverInfo> GetDriverInfos()
        {
            return _configServer.DriverInfos;
        }
        [ActionName("Project")]
        [HttpPut]
        public RestAPIResult PutProject([FromBody] ProjectConfig projectConfig)
        {
            if (projectConfig == null)
            {
                return RestAPIResult.FAIL;
            }
            _configServer.Config = projectConfig;
            return RestAPIResult.OK;
        }
        [ActionName("DriverInformation")]
        [HttpPost]
        public UpDllFileResult UpDriverDll(string fileName, [FromBody] Stream stream)
        {
            var infors = _configServer.SaveDriverDll(fileName, stream, out string msg);
            if (infors != null)
            {
                return new UpDllFileResult { DriverInfos = infors, Result = RestAPIResult.OK };
            }
            else
            {
                return new UpDllFileResult { Result = RestAPIResult.OK, ErrorMsg = msg };

            }
        }
    }
}
