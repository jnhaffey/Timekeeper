using System;
using System.Collections.Generic;
using System.Linq;

namespace Timekeeper.SettingsTypes
{
    public class ProjectSettingsCollection
    {
        public string CrmConnectionString { get; set; }
        public string DefaultActiveState { get; set; }
        public string DefaultPausedState { get; set; }
        public bool Whitelist { get; set; }
        public List<string> WhitelistedProjects { get; set; }
        public List<string> BlacklistedProjects { get; set; }
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
        public bool IsIncluded(string projectName)
        {
            if (Whitelist)
            {
                return WhitelistedProjects.Any(x => x.Equals(projectName, StringComparison.InvariantCultureIgnoreCase)) && !BlacklistedProjects.Any(x => x.Equals(projectName, StringComparison.InvariantCultureIgnoreCase));
            }
            else
            {
                return !BlacklistedProjects.Any(x => x.Equals(projectName, StringComparison.InvariantCultureIgnoreCase));
            }
        }
    }
}