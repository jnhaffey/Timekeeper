using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TeamFoundation;

namespace Microsoft.ALMRangers.Samples.MyHistory
{
    public static class Global
    {
        private static string _projectName;

        public static string ProjectName
        {
            get
            {
                if (_projectName == null)
                {
                    var dte = Package.GetGlobalService(typeof(EnvDTE.DTE)) as EnvDTE.DTE;
                    TeamFoundationServerExt ext = dte.GetObject("Microsoft.VisualStudio.TeamFoundation.TeamFoundationServerExt") as TeamFoundationServerExt;
                    _projectName = ext.ActiveProjectContext == null ? null : ext.ActiveProjectContext.ProjectName;

                }
                return _projectName;
            }
            set
            {
                _projectName = value;
            }
        }
    }
}