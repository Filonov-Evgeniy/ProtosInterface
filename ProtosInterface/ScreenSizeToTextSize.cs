using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ProtosInterface
{
    public class ScreenSizeToTextSize : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double screenSize = System.Convert.ToDouble(value);
            double textSize;
            if (screenSize <= 450)
            {
                textSize = 14;
            }
            else
            {
                textSize = Math.Round(14 + (screenSize - 450) / 75, 2);
            }
            return textSize;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
