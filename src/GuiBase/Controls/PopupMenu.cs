using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace GuiBase.Controls
{
    public class PopupMenu : ContextMenu
    {

        public PopupMenu() 
        {
           
        }

        public MenuItem AddMenuItem(string header,ICommand command,object icon)
        {
            var item = new MenuItem();
            item.Header = header;
            item.Command = command;
            item.Icon = icon;
            Items.Add(item);
            return item;
        }
        public MenuItem AddMenuItem(string header, ICommand command)
        {
          return  AddMenuItem(header, command, null);
        }
        public Separator AddMenuSeparatorItem()
        {
            var sp = new Separator();
            //sp.Foreground = new SolidColorBrush(Colors.AliceBlue);
            sp.Width = 2;
            Items.Add(sp);
            return sp;
        }
    }
}
