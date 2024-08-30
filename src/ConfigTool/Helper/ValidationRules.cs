using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Utillity.Data;

namespace ConfigTool.Helper
{
    public class ChannelNameValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (RegexCheck.IsString(value.ToString()))
            {
                return new ValidationResult(true, "");
            }
            else
            {
                return new ValidationResult(false, "Channel Name include special character ");
            }
        }
    }
}
