using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataServer.Permission
{
    public interface IResourceManager
    {
        Resource GetResourceById(string id);

        Resource GetResourceByName(string name);
        List<Resource> GetAllResources();

        bool CreateResource(Resource resource);

        bool UpdateResource(Resource resource);

        bool DeleteResource(Resource resource);
    }
}
