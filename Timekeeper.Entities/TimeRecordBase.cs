using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Timekeeper.Entities
{
    public abstract class TimeRecordBase : INotifyPropertyChanged
    {
        private DateTime _endTime;
        private DateTime _startTime;
        private string _order;
        private string _case;
        private string _itemTitle;
        private int _startRevision;
        private int _endRevision;
        private int _splitNumber;

        public event PropertyChangedEventHandler PropertyChanged;

        public abstract bool IsExported
        {
            get;
        }

        public abstract bool IsIgnored
        {
            get;
        }

        public abstract bool CanExport
        {
            get;
        }

        public abstract bool CanIgnore
        {
            get;
        }

        public abstract void SetExported();

        public abstract void SetIgnored();

        public int EndRevision
        {
            get { return _endRevision; }
            set { _endRevision = value; Raise("EndRevision", "IsExported", "IsIgnored", "IsIgnoredOrExported"); }
        }

        public int StartRevision
        {
            get { return _startRevision; }
            set { _startRevision = value; Raise("StartRevision", "IsExported", "IsIgnored", "IsIgnoredOrExported"); }
        }

        public virtual string ItemTitle
        {
            get { return _itemTitle; }
            set { _itemTitle = value; }
        }

        public virtual string Order
        {
            get { return _order; }
            set { _order = value; }
        }

        public virtual string Case
        {
            get { return _case; }
            set { _case = value; }
        }

        public TimeSpan Duration
        {
            get
            {
                return EndTime - StartTime;
            }
        }

        public int SplitNumber
        {
            get { return _splitNumber; }
            set { _splitNumber = value; Raise("SplitNumber", "IsExported", "IsIgnored", "IsIgnoredOrExported"); }
        }

        public DateTime StartTime
        {
            get { return _startTime; }
            set { if (_startTime != value) { _startTime = value; Raise("StartTime", "Duration", "StartMinuteOfDay", "TotalMinutes"); } }
        }

        public DateTime EndTime
        {
            get { return _endTime; }
            set { if (_endTime != value) { _endTime = value; Raise("EndTime", "Duration", "EndMinuteOfDay", "TotalMinutes"); } }
        }

        public int StartMinuteOfDay
        {
            get
            {
                return (int)Math.Floor(StartTime.TimeOfDay.TotalMinutes);
            }
            set
            {
                StartTime = StartTime.Date.AddMinutes(value);
            }
        }

        public int EndMinuteOfDay
        {
            get
            {
                return (int)Math.Floor(EndTime.TimeOfDay.TotalMinutes);
            }
            set
            {
                EndTime = EndTime.Date.AddMinutes(value);
            }
        }

        public int TotalMinutes
        {
            get
            {
                return (int)Math.Floor(Duration.TotalMinutes);
            }
        }

        public List<TimeRecordBase> SplitToRecordPerDay()
        {
            TimeSpan startTime = new TimeSpan(0, 0, 0);
            TimeSpan endTime = new TimeSpan(23, 59, 59);
            var recs = new List<TimeRecordBase>();
            var finalTime = _endTime.TimeOfDay > endTime ? _endTime.Date.Add(endTime) : _endTime;
            var currentTime = _startTime.TimeOfDay < startTime ? _startTime.Date.Add(startTime) : _startTime;
            TimeRecordBase rec;
            int splitNumber = 0;

            do
            {
                rec = this.Clone();
                rec.SplitNumber = splitNumber;
                splitNumber++;

                if (currentTime.TimeOfDay < startTime) //Crop start time
                {
                    rec._startTime = currentTime.Date.Add(startTime);
                }
                else //Use the start time as-is
                {
                    rec._startTime = currentTime;
                }

                if (_endTime.Date > rec._startTime.Date) //Split the record
                {
                    currentTime = rec._startTime.Date.AddDays(1).Add(startTime);
                    rec._endTime = rec._startTime.Date.Add(endTime);
                }
                else //Complete the record
                {
                    rec._endTime = finalTime;
                }
                recs.Add(rec);
            }
            while (rec.EndTime != finalTime);

            return recs;
        }

        protected abstract TimeRecordBase Clone();

        public void Crop(TimeSpan startTime, TimeSpan endTime)
        {
            if (StartTime.TimeOfDay < startTime && EndTime.TimeOfDay > startTime)
            {
                StartTime = StartTime.Date.Add(startTime);
            }
            if (EndTime.TimeOfDay > endTime && StartTime.TimeOfDay < endTime)
            {
                EndTime = EndTime.Date.Add(endTime);
            }
        }

        public bool Overlaps(TimeRecordBase record)
        {
            return this != record && (
            (record.StartTime < StartTime && record.EndTime > EndTime) || //encompasses this record
            (record.StartTime < EndTime && record.StartTime > StartTime) || //starts in this record
            (record.EndTime < EndTime && record.EndTime > StartTime)); //ends in this record
        }

        public bool OverlapsAny(IEnumerable<TimeRecordBase> record)
        {
            return record.Any(x => Overlaps(x));
        }

        protected void Raise(params string[] p)
        {
            if (PropertyChanged != null)
            {
                foreach (var prop in p)
                {
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs(prop));
                }
            }
        }
    }
}