

using ALT.CVL.Common.Interface;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ALT.CVL.Controls.Dictionary
{
    public partial class DictionaryUC
    {
        private void TextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key == System.Windows.Input.Key.Enter)
            {
                TextBox tb = sender as TextBox;
                BindingExpression be = tb.GetBindingExpression(TextBox.TextProperty);
                be.UpdateSource();
            }
        }
    }

    public class ParameterTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            //select template by parameter type

            ICamParameter parameter = item as ICamParameter;
            FrameworkElement ei = container as FrameworkElement;

            if (parameter.Type == typeof(System.Enum))
                return (DataTemplate)ei.FindResource("enumParameterTemplate");
            else if(parameter.Type == typeof(bool))
                return (DataTemplate)ei.FindResource("booleanParameterTemplate");
            else
                return (DataTemplate)ei.FindResource("commonParameterTemplate");

            return null;
        }
    }

    public class ReverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                return !(bool)value;
            }
            else
                return null;
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
