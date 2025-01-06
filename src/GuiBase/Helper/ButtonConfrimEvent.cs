using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Events;
using Prism.Services.Dialogs;

namespace ConfigTool.Models
{
    public class ButtonConfrimEvent : PubSubEvent<ButtonResult>
    {
        public void Clear()
        {
            if (Subscriptions is List<IEventSubscription> sb)
            {
                for (var i = sb.Count - 1; i >= 0; i--)
                {
                    sb.RemoveAt(i);
                }
            }
          
        }
    }
}
