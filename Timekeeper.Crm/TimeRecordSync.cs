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
            //var conn = new Microsoft.Xrm.Client.CrmConnection("Xrm");
            //var creds = conn.ClientCredentials;

            Xrm.XrmServiceContext context = new Xrm.XrmServiceContext("Xrm");

            var me = context.SystemUserSet.Where(x => x.LastName == "Mann").ToList().First();
            var timeslots = me.user_new_timeslot;
        }
    }
}
