using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Panacea.Modules.Billing.Converters
{
    [ValueConversion(typeof(bool), typeof(System.Windows.Media.LinearGradientBrush))]
    public class IsFeaturedToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            System.Windows.Media.LinearGradientBrush brush = null;
            if ((bool)value)
            {
                brush = new System.Windows.Media.LinearGradientBrush();
                brush.StartPoint = new System.Windows.Point(0, 0);
                brush.EndPoint = new System.Windows.Point(1, 0);

                // Create and add Gradient stops
                GradientStop blueGS = new GradientStop();
                blueGS.Color = System.Windows.Media.Color.FromArgb(255, 255, 209, 111);
                blueGS.Offset = 0.0;
                brush.GradientStops.Add(blueGS);

                GradientStop blueGS1 = new GradientStop();
                blueGS1.Color = System.Windows.Media.Color.FromArgb(255, 245, 200, 112);
                blueGS1.Offset = 1.0;
                brush.GradientStops.Add(blueGS1);
            }
            else
            {
                brush = new System.Windows.Media.LinearGradientBrush();
                brush.StartPoint = new System.Windows.Point(0, 0);
                brush.EndPoint = new System.Windows.Point(1, 0);

                // Create and add Gradient stops
                GradientStop blueGS = new GradientStop();
                blueGS.Color = System.Windows.Media.Color.FromArgb(255, 239, 239, 239);
                blueGS.Offset = 0.0;
                brush.GradientStops.Add(blueGS);

                GradientStop blueGS1 = new GradientStop();
                blueGS1.Color = System.Windows.Media.Color.FromArgb(255, 245, 245, 245);
                blueGS1.Offset = 1.0;
                brush.GradientStops.Add(blueGS1);

            }
            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
