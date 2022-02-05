using Cognex.VisionPro3D;
using System;
using System.Globalization;
using System.Windows.Data;

namespace ALT.BoltHeight.UI.Dictionaries
{
    [ValueConversion(typeof(string), typeof(Cog3DRangeImageCrossSectionPointTypeConstants))]
    public class StringToPointTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(value.ToString()))
                return null;

            if (Enum.TryParse(value.ToString(), out Cog3DRangeImageCrossSectionPointTypeConstants en))
                return en;

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
