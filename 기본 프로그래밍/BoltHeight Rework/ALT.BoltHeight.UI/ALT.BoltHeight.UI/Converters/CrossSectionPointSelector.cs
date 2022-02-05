using System;
using System.Globalization;
using System.Windows.Data;

namespace ALT.BoltHeight.UI.Dictionaries
{
    public class CrossSectionPointSelector : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
                return value;

            if (!value.ToString().Contains("ExtractPoint"))
                return true;

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class CrossSectionLineSegmentSelector : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
                return value;

            if (!value.ToString().Contains("ExtractLineSegment"))
                return true;

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
