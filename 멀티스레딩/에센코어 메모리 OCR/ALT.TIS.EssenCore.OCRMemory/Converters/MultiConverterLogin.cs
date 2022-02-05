using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ALT.TIS.EssenCore.OCRMemory.Converters
{
    public class MultiConverterLogin : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            LoginParameter loginParameter = new LoginParameter();
            foreach (Control item in values)
            {
                loginParameter.PWBox = values.Where(x => x.GetType().Equals(typeof(PasswordBox))).FirstOrDefault() as PasswordBox;
                loginParameter.WindowLogin = values.Where(x => x.GetType().Equals(typeof(ALT.TIS.EssenCore.OCRMemory.Views.WdAccess))).FirstOrDefault() as Window;
            }
            return loginParameter;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new object[2];
        }
    }

    public class LoginParameter
    {
        public PasswordBox PWBox { get; set; }
        public Window WindowLogin { get; set; }
    }
}
