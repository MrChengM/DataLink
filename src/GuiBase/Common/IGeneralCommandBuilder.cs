﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuiBase.Common
{
  public interface IGeneralCommandBuilder
    {
        Dictionary<string, GeneralCommand> GeneralCommands { get; }
    }
}
