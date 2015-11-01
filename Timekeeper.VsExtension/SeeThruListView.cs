using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Timekeeper.VsExtension
{
    public sealed class SeeThruListView : ListView
    {
        public SeeThruListView()
        {
            this.PreviewMouseWheel += SeeThruListView_PreviewMouseWheel;
        }

        void SeeThruListView_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;
            var e2 = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
            e2.RoutedEvent = UIElement.MouseWheelEvent;
            RaiseEvent(e2);
        }
    }
}