using System;

namespace Timekeeper.Entities
{
    public class BasicTimeRecord : TimeRecordBase
    {
        public override bool IsExported
        {
            get { return false; }
        }

        public override bool IsIgnored
        {
            get { return false; }
        }

        public override bool CanExport
        {
            get { return false; }
        }

        public override bool CanIgnore
        {
            get { return false; }
        }

        public override void SetExported()
        {
            throw new NotImplementedException();
        }

        public override void SetIgnored()
        {
            throw new NotImplementedException();
        }

        protected override TimeRecordBase Clone()
        {
            return new BasicTimeRecord()
            {
                StartTime = this.StartTime,
                EndTime = this.EndTime,
                StartRevision = this.StartRevision,
                EndRevision = this.EndRevision,
                ItemTitle = this.ItemTitle,
                Case = this.Case,
                Order = this.Order,
                SplitNumber = this.SplitNumber
            };
        }
    }
}