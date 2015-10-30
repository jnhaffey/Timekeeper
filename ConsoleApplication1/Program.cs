using Microsoft.ALMRangers.Samples.MyHistory;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            //Timekeeper.SettingsTypes.ProjectStateNamesCollection psnc = new Timekeeper.SettingsTypes.ProjectStateNamesCollection();
            //psnc.Projects = new List<Timekeeper.SettingsTypes.ProjectStateNames>();
            //psnc.DefaultActiveState = "Active";
            //psnc.DefaultPausedState = "New";
            //psnc.Projects.Add(new Timekeeper.SettingsTypes.ProjectStateNames() { ProjectName = "Consol", ActiveState = "Active", PausedState = "Paused" });
            //psnc.Projects.Add(new Timekeeper.SettingsTypes.ProjectStateNames() { ProjectName = "Dan M Test Thing", ActiveState = "Active", PausedState = "Paused" });
            //System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(psnc.GetType());
            //x.Serialize(File.Create("E:\\output.xml"), psnc);
            //Console.ReadLine();

            TimeRecord t = new TimeRecord()
            {
                StartTime = DateTime.Now.AddDays(-7),
                EndTime = DateTime.Now.AddDays(-2),
                StartRevision = 0,
                EndRevision = 2
            };
            var ts = t.SplitToRecordPerDay();
            ts.ForEach(x => x.Crop(new TimeSpan(9,0,0), new TimeSpan(18,0,0)));
        }
    }
}
