using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ALT.ResultViewer
{
    public class FontSizeConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double product = 1;

            foreach (double value in values)
            {
                product *= value;
            }

            var returnFontSize = Math.Ceiling(Math.Sqrt(product) / 25);

            return returnFontSize == 0 ? 1 : returnFontSize;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
