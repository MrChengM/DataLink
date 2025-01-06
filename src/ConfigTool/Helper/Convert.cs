using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ConfigTool.Models;

namespace ConfigTool.Helper
{
    //public class ConvertToComBoxItem : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        if (value == null)
    //            return DependencyProperty.UnsetValue;
    //        switch ((NodeType)value)
    //        {
    //            case NodeType.Channel:
    //                return "Collapsed";
    //            case NodeType.Client:
    //                return "Collapsed";


    //            default:
    //                return "Visible";


    //        }
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        var temp = value as ComboBoxItem;
    //        if (temp != null)
    //        {
    //            return temp.Content;
    //        }
    //        return DependencyProperty.UnsetValue;
    //    }
    //}

}
