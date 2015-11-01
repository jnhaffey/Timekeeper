using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Timekeeper.Entities;

namespace Timekeeper.Timeline
{
    /// <summary>
    /// Interaction logic for TimeslotControl.xaml
    /// </summary>
    public partial class TimeslotControl : UserControl
    {
        private bool _holdingStart, _holdingEnd;

        public bool HoldingEnd
        {
            get { return _holdingEnd; }
            set { _holdingEnd = value; }
        }

        public bool HoldingStart
        {
            get { return _holdingStart; }
            set { _holdingStart = value; }
        }
        private double _tickSize = 2, _origin = 0, _originalMinutes = 0;

        public TimeslotControl()
        {
            InitializeComponent();
        }

        public WorkItemTimeRecord Model
        {
            get
            {
                return (WorkItemTimeRecord)DataContext;
            }
            set
            {
                DataContext = value;
            }
        }



        private void UserControl_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                var pos = e.GetPosition(Application.Current.MainWindow);
                if (_holdingStart)
                {
                    var delta = (pos.X - _origin) / _tickSize;
                    Model.StartTime = Model.StartTime.Date.AddMinutes(_originalMinutes).AddMinutes(delta);
                }
                else if (_holdingEnd)
                {
                    var delta = (pos.X - _origin) / _tickSize;
                    Model.EndTime = Model.EndTime.Date.AddMinutes(_originalMinutes).AddMinutes(delta);
                }
            }
            catch 
            { 
            }

            if (PointIsAtEdge(e.GetPosition(this)))
            {
                this.Cursor = Cursors.SizeWE;
            }
            else
            {
                this.Cursor = Cursors.Arrow;
            }
        }

        public bool PointIsAtEdge(Point pos)
        {
            if (pos.X < 2 || pos.X > this.ActualWidth - 2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_holdingStart || _holdingEnd)
            {
                return;
            }
            var pos = e.GetPosition(this);
            if (pos.X < (_tickSize * 2))
            {
                _holdingStart = true;
                _tickSize = this.ActualWidth / Model.Duration.TotalMinutes;
                _originalMinutes = Model.StartTime.TimeOfDay.TotalMinutes;
                _origin = e.GetPosition(Application.Current.MainWindow).X;
                Mouse.Capture(this);
            }
            else if (pos.X > this.ActualWidth - (_tickSize * 2))
            {
                _holdingEnd = true;
                _tickSize = this.ActualWidth / Model.Duration.TotalMinutes;
                _originalMinutes = Model.EndTime.TimeOfDay.TotalMinutes;
                _origin = e.GetPosition(Application.Current.MainWindow).X;
                Mouse.Capture(this);
            }
        }

        private void UserControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _holdingStart = _holdingEnd = false;
            Mouse.Capture(null);
            this.Cursor = Cursors.Arrow;
        }
    }
}
