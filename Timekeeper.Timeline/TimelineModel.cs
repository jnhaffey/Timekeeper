using Microsoft.ALMRangers.Samples.MyHistory;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Timekeeper.Timeline
{
    public class TimelineModel : INotifyPropertyChanged
    {
        private ObservableCollection<TimelineLaneModel> _lanes = new ObservableCollection<TimelineLaneModel>();

        private ObservableCollection<TimeRecord> _records;

        public TimelineModel(DayOfWeek day)
        {
            _day = day;
            Records = new ObservableCollection<TimeRecord>();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private DayOfWeek _day;
        
        public string DayName
        {
            get
            {
                return Enum.GetName(typeof(DayOfWeek), _day);
            }
        }

        public DayOfWeek Day
        {
            get
            {
                return _day;
            }
            set
            {
                _day = value;
                RaisePropertyChanged("Day", "DayName", "DayNumber");
            }
        }
        
        public int DayNumber
        {
            get
            {
                return (int)Day;
            }
        }

        public int LaneCount
        {
            get
            {
                return _lanes.Count;
            }
        }

        public ObservableCollection<TimelineLaneModel> Lanes
        {
            get
            {
                return _lanes;
            }
            private set
            {
                _lanes = value;
                RaisePropertyChanged("Lanes", "LaneCount");
            }
        }

        public ObservableCollection<TimeRecord> Records
        {
            get
            {
                return _records;
            }
            set
            {
                if (_records != null)
                {
                    _records.CollectionChanged -= _records_CollectionChanged;
                }
                _records = value;
                if (_records != null)
                {
                    if (_records.Count > 0 && _records.First().StartTime.Date != _records.First().EndTime.Date || _records.Any(x => x.StartTime.Date != _records.First().StartTime.Date || x.EndTime.Date != _records.First().EndTime.Date))
                    {
                        throw new InvalidOperationException("Only a single days' events can be placed on a timeline");
                    }
                    if (_records.Count > 0)
                    {
                        if (_day != _records.First().StartTime.Date.DayOfWeek)
                        {
                            throw new InvalidOperationException("Only events for " + Enum.GetName(typeof(DayOfWeek), _day) + " can be put on this timeline");
                        }
                    }
                    _records.CollectionChanged += _records_CollectionChanged;
                }
                _records.ToList().ForEach(x => x.PropertyChanged += x_PropertyChanged);
                RaisePropertyChanged("Records");
                CalculateLanes();
            }
        }

        void x_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RecalculateLanes();
        }

        private bool _recalculatingLanes = false;

        private void _records_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (_records.Count > 0 && _records.First().StartTime.Date != _records.First().EndTime.Date || _records.Any(x => x.StartTime.Date != _records.First().StartTime.Date || x.EndTime.Date != _records.First().EndTime.Date))
            {
                throw new InvalidOperationException("Only a single days' events can be placed on a timeline");
            }
            if (_records.Count > 0)
            {
                if (_day != _records.First().StartTime.Date.DayOfWeek)
                {
                    throw new InvalidOperationException("Only events for " + Enum.GetName(typeof(DayOfWeek), _day) + " can be put on this timeline");
                }
            }

            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove || e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset || e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
            {
                foreach (var x in e.OldItems.Cast<TimeRecord>())
                {
                    x.PropertyChanged -= x_PropertyChanged;
                }
            }

            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add || e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
            {
                foreach(var x in e.NewItems.Cast<TimeRecord>())
                {
                    x.PropertyChanged += x_PropertyChanged;
                }
            }
           
            RaisePropertyChanged("Records");
            CalculateLanes();
        }

        private void RecalculateLanes()
        {
            if (_recalculatingLanes)
            {
                return;
            }
            _recalculatingLanes = true;
            var currentlyDragging = App.Current.MainWindow.FindVisualChildren<TimeslotControl>().Where(x => x.HoldingStart || x.HoldingEnd).Select(x => x.Model);

            foreach(var lane in Lanes.ToList())
            {
                var overlappers = lane.Items.Where(x => x.OverlapsAny(lane.Items)).Except(currentlyDragging).ToList();
                while (overlappers.Count > 0)
                {
                    lane.Items.Remove(overlappers.First());
                    var newLane = new TimelineLaneModel() { LaneNumber = Lanes.Count + 1 };
                    newLane.Items.Add(overlappers.First());
                    Lanes.Add(newLane);
                    overlappers = lane.Items.Where(x => x.OverlapsAny(lane.Items)).ToList();
                }
            }

            foreach(var lane in Lanes.OrderByDescending(x => x.LaneNumber).ToList())
            {
                foreach (var item in lane.Items.ToList())
                {
                    foreach(var lane2 in Lanes.Where(x => x.LaneNumber < lane.LaneNumber).ToList())
                    {
                        if (!item.OverlapsAny(lane2.Items))
                        {
                            if (currentlyDragging.Contains(item))
                            {
                                foreach(var item2 in lane2.Items.ToList())
                                {
                                    lane.Items.Add(item2);
                                    lane2.Items.Remove(item2);
                                }
                            }
                            else
                            {
                                lane2.Items.Add(item);
                                lane.Items.Remove(item);
                            }
                        }
                    }
                }
            }

            foreach (var lane in Lanes.ToList())
            {
                if (lane.Items.Count == 0)
                {
                    Lanes.Remove(lane);
                }
            }

            foreach (var lane in Lanes)
            {
                lane.LaneNumber = Lanes.IndexOf(lane);
            }

            RaisePropertyChanged("Lanes");
            _recalculatingLanes = false;
        }

        private void CalculateLanes()
        {
            var lanes = new List<TimelineLaneModel>();
            foreach (var record in Records)
            {
                var added = false;
                foreach (var lane in lanes)
                {
                    if (!record.OverlapsAny(lane.Items))
                    {
                        lane.Items.Add(record);
                        added = true;
                        break;
                    }
                }
                if (!added)
                {
                    var lane = new TimelineLaneModel();
                    lane.LaneNumber = lanes.Count;
                    lane.LaneName = string.Format("Lane {0}", lanes.Count);
                    lane.Items.Add(record);
                    lanes.Add(lane);
                }
            }
            Lanes = new ObservableCollection<TimelineLaneModel>(lanes);
        }

        private void RaisePropertyChanged(params string[] names)
        {
            if (PropertyChanged != null)
            {
                foreach (var name in names)
                {
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
                }
            }
        }
    }
}
