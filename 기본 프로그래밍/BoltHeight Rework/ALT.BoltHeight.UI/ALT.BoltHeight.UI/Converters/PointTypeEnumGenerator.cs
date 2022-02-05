using Cognex.VisionPro3D;
using System;
using System.Globalization;
using System.Windows.Data;

namespace ALT.BoltHeight.UI.Dictionaries
{
    public class PointTypeEnumGenerator : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Enum.GetNames(typeof(Cog3DRangeImageCrossSectionPointTypeConstants));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
