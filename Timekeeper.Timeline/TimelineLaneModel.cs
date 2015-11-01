using Microsoft.ALMRangers.Samples.MyHistory;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace Timekeeper.Timeline
{
    public class TimelineLaneModel : INotifyPropertyChanged
    {
        private int _laneNumber;
        private ObservableCollection<TimeRecord> _items;
        public TimelineLaneModel()
        {
            Items = new ObservableCollection<TimeRecord>();
        }

        public int LaneNumber
        {
            get
            {
                return _laneNumber;
            }
            set
            {
                _laneNumber = value;
                RaisePropertyChanged("LaneNumber");
            }
        }

        public string LaneName
        {
            get
            {
                return _laneName;
            }
            set
            {
                _laneName = value;
                RaisePropertyChanged("LaneName");
            }
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

        public ObservableCollection<TimeRecord> Items
        {
            get
            {
                return _items;
            }
            set
            {
                if (_items != null)
                {
                    _items.CollectionChanged -= _items_CollectionChanged;
                }
                _items = value;
                if (_items != null)
                {
                    _items.CollectionChanged += _items_CollectionChanged;
                }
                RaisePropertyChanged("Items");
            }
        }

        void _items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged("Items");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private string _laneName;
    }

}