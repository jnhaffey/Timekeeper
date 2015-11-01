using Microsoft.ALMRangers.Samples.MyHistory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Timekeeper.Crm
{
    public class TimeRecordSync
    {
        public TimeRecordSync(IEnumerable<TimeRecord> records)
        {
            Xrm.XrmServiceContext context = new Xrm.XrmServiceContext("Xrm");
            var me = context.SystemUserSet.Where(x => x.LastName == "Mann").ToList();
        }
    }
}
