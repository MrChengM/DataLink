using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utillity.Data
{
    public static class DelegateFunction
    {
        public static int SubsCounts(Delegate handler)
        {
            if (handler == null)
            {
                return 0;
            }
            var dels = handler.GetInvocationList();
            return dels.Length;
        }
        public static void ClearAll(Delegate handler)
        {
            if (handler == null)
            {
                return ;
            }
            var dels = handler.GetInvocationList();
            foreach (var del in dels)
            {
                Delegate.RemoveAll(handler,del);
            }
        }
    }
}
