using Timekeeper.VsExtension.Properties;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Timekeeper.VsExtension
{
    public class StateToDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
        object parameter, CultureInfo culture)
        {
            if (value is WorkItem)
            {
                if ((value as WorkItem).State == Properties.Settings.Default.SettingsCollection.GetActiveState((value as WorkItem).Project.Name))
                {
                    return "Currently Working";
                }
                else
                {
                    return "Currently Paused";
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