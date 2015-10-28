using Company.Timekeeper.Properties;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Microsoft.ALMRangers.Samples.MyHistory
{
    public class StateToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
        object parameter, CultureInfo culture)
        {
            if (value is WorkItem)
            {
                if ((value as WorkItem).State == Settings.Default.StateNameConfiguration.GetActiveState((value as WorkItem).Project.Name))
                {
                    return new SolidColorBrush(Colors.DarkGreen);
                }
                else
                {
                    return new SolidColorBrush(Colors.DarkRed);
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType,
        object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}