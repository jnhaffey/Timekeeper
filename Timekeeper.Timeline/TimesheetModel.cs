using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Timekeeper.Entities;

namespace Timekeeper.Timeline
{
    public class TimesheetModel : INotifyPropertyChanged
    {
        private ObservableCollection<TimelineModel> _days = new ObservableCollection<TimelineModel>();

        public TimesheetModel(IEnumerable<TimeRecordBase> records)
        {
            _days.CollectionChanged += _days_CollectionChanged;
            SetRecordsInternal(records);
        }

        private void SetRecordsInternal(IEnumerable<TimeRecordBase> records)
        {
            foreach (var val in Enum.GetValues(typeof(DayOfWeek)))
            {
                var line = new TimelineModel((DayOfWeek)val);
                foreach (var record in records.Where(x => x.StartTime.DayOfWeek == (DayOfWeek)val))
                {
                    line.Records.Add(record);
                }
                _days.Add(line);
            }
        }

        public IEnumerable<TimeRecordBase> Records
        {
            get
            {
                return _days.SelectMany(x => x.Records);
            }
            set
            {
                SetRecordsInternal(value);
            }
        }

        private void _days_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged("Days");
        }

        public TimelineModel this[DayOfWeek i]
        {
            get
            {
                return Days[(int)i];
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ReadOnlyObservableCollection<TimelineModel> Days
        {
            get
            {
                return new ReadOnlyObservableCollection<TimelineModel>(_days);
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
    }
}