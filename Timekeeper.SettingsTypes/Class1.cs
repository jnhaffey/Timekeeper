using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Timekeeper.SettingsTypes
{
    public class ProjectStateNamesCollection
    {
        public string DefaultActiveState { get; set; }
        public string DefaultPausedState { get; set; }
        public List<ProjectStateNames> Projects { get; set; }
        public string GetActiveState(string projectName)
        {
            var prj = Projects.FirstOrDefault(x => x != null && x.ProjectName != null && x.ProjectName.Equals(projectName, StringComparison.InvariantCultureIgnoreCase));
            return prj == null ? DefaultActiveState : prj.ActiveState;
        }
        public string GetPausedState(string projectName)
        {
            var prj = Projects.FirstOrDefault(x => x != null && x.ProjectName != null && x.ProjectName.Equals(projectName, StringComparison.InvariantCultureIgnoreCase));
            return prj == null ? DefaultPausedState : prj.PausedState;
        }
    }

    public class ProjectStateNames
    {
        public string ProjectName { get; set; }
        public string ActiveState { get; set; }
        public string PausedState { get; set; }
    }
}
