using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Timekeeper.Crm;
using Timekeeper.Entities;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            //Timekeeper.SettingsTypes.ProjectSettingsCollection psnc = new Timekeeper.SettingsTypes.ProjectSettingsCollection();
            //psnc.Projects = new List<Timekeeper.SettingsTypes.ProjectStateNames>();
            //psnc.DefaultActiveState = "Active";
            //psnc.DefaultPausedState = "New";
            //psnc.Projects.Add(new Timekeeper.SettingsTypes.ProjectStateNames() { ProjectName = "Consol", ActiveState = "Active", PausedState = "Paused" });
            //psnc.Projects.Add(new Timekeeper.SettingsTypes.ProjectStateNames() { ProjectName = "Dan M Test Thing", ActiveState = "Active", PausedState = "Paused" });

            //var whitelist = new List<string>();
            //whitelist.Add("Consol");
            //whitelist.Add("Dan M Test Thing");
            //psnc.Whitelist = true;
            //psnc.WhitelistedProjects = whitelist;

            //System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(psnc.GetType());
            //x.Serialize(File.Create("E:\\output.xml"), psnc);
            //Console.ReadLine();

            //TimeRecord t = new TimeRecord()
            //{
            //    StartTime = DateTime.Now.AddDays(-7),
            //    EndTime = DateTime.Now.AddDays(-2),
            //    StartRevision = 0,
            //    EndRevision = 2
            //};
            //var ts = t.SplitToRecordPerDay();
            //ts.ForEach(x => x.Crop(new TimeSpan(9,0,0), new TimeSpan(18,0,0)));
            //var ls = new List<TimeRecordBase>();
            //ls.Add(new BasicTimeRecord()
            //    {
            //        StartTime = DateTime.Now,
            //        EndTime = DateTime.Now.AddHours(4)
            //    });
            //new TimeRecordSync("");

            var tr = new TimeRecordSync(@"Server=https://xrm.felinesoft.com; Username=felinesoft\Dan.Mann; Password=Wangimg1223Apr");
        }
    }
}
