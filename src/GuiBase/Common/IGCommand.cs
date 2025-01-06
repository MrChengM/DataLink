using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GuiBase.Common
{
    public interface IGCommand
    {
        string Name { get; set; }
        string Type { get; set; }

        string TranslationId { get; set; }
        bool HasPermission { get; }

        bool Enable { get; }

        ICommand Command { get; set; }
    }
}
