using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.Win32;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using EnvDTE80;
using System.Linq;
using Microsoft.VisualStudio.TeamFoundation.VersionControl;
using Microsoft.VisualStudio.TeamFoundation;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using System.Collections.Generic;
using Company.Timekeeper.Properties;
using Microsoft.ALMRangers.Samples.MyHistory;

namespace Company.Timekeeper
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    ///
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// </summary>
    // This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
    // a package.
    [PackageRegistration(UseManagedResourcesOnly = true)]
    // This attribute is used to register the information needed to show this package
    // in the Help/About dialog of Visual Studio.
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [Guid(GuidList.guidTimekeeperPkgString)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.NoSolution_string)]
    public sealed class TimekeeperPackage : Package, IVsSolutionEvents
    {
        private uint _cookie;

        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require 
        /// any Visual Studio service because at this point the package object is created but 
        /// not sited yet inside Visual Studio environment. The place to do all the other 
        /// initialization is the Initialize method.
        /// </summary>
        public TimekeeperPackage()
        {
            Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));
        }



        /////////////////////////////////////////////////////////////////////////////
        // Overridden Package Implementation
        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            var dte = Package.GetGlobalService(typeof(EnvDTE.DTE)) as EnvDTE.DTE;
            TeamFoundationServerExt ext = dte.GetObject("Microsoft.VisualStudio.TeamFoundation.TeamFoundationServerExt") as TeamFoundationServerExt;
            var slnSvc = GetService(typeof(SVsSolution)) as IVsSolution;
            uint cookie;
            slnSvc.AdviseSolutionEvents(this, out cookie);
            _cookie = cookie;
            ext.ProjectContextChanged += ext_ProjectContextChanged;
            ext_ProjectContextChanged(null, null);
        }

        private string _projectName = string.Empty;

        void ext_ProjectContextChanged(object sender, EventArgs e)
        {
            var dte = Package.GetGlobalService(typeof(EnvDTE.DTE)) as EnvDTE.DTE;
            TeamFoundationServerExt ext = dte.GetObject("Microsoft.VisualStudio.TeamFoundation.TeamFoundationServerExt") as TeamFoundationServerExt;
            var vsExt = dte.GetObject("Microsoft.VisualStudio.TeamFoundation.VersionControl.VersionControlExt") as VersionControlExt;
            VersionControlServer vcs = vsExt.SolutionWorkspace == null ? null : vsExt.SolutionWorkspace.VersionControlServer;
            Global.ProjectName = ext.ActiveProjectContext == null ? null : ext.ActiveProjectContext.ProjectName;

            if (vcs != null)
            {
                vcs.UnshelveShelveset -= vcs_UnshelveShelveset;
                vcs.CommitShelveset -= vcs_CommitShelveset;
                vcs.UnshelveShelveset += vcs_UnshelveShelveset;
                vcs.CommitShelveset += vcs_CommitShelveset;
            }
        }

        void vcs_CommitShelveset(object sender, CommitShelvesetEventArgs e)
        {
            var shelf = e.Shelveset;

            if (e.Shelveset.Properties.Any(x => x.PropertyName == "Microsoft.TeamFoundation.VersionControl.Shelveset.CreatedBy" && (string)x.Value == "Suspend"))
            {
                foreach (var item in shelf.WorkItemInfo)
                {
                    if (item.WorkItem.State == Settings.Default.StateNameConfiguration.GetActiveState(_projectName))
                    {
                        item.WorkItem.PartialOpen();
                        item.WorkItem.State = Settings.Default.StateNameConfiguration.GetPausedState(_projectName);
                        item.WorkItem.Reason = "My Work Suspended";
                        item.WorkItem.Save();
                    }
                }
            }
        }

        void vcs_UnshelveShelveset(object sender, UnshelveShelvesetEventArgs e)
        {
            var shelf = e.Shelveset;
            foreach (var item in shelf.WorkItemInfo)
            {
                if (item.WorkItem.State != Settings.Default.StateNameConfiguration.GetActiveState(_projectName))
                {
                    item.WorkItem.PartialOpen();
                    item.WorkItem.State = Settings.Default.StateNameConfiguration.GetActiveState(_projectName);
                    item.WorkItem.Reason = "My Work Resumed";
                    item.WorkItem.Save();
                }
            }
        }
        #endregion


        public int OnAfterCloseSolution(object pUnkReserved)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterLoadProject(IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
        {
            ext_ProjectContextChanged(null, null);
            return VSConstants.S_OK;
        }

        public int OnBeforeCloseProject(IVsHierarchy pHierarchy, int fRemoved)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeCloseSolution(object pUnkReserved)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeUnloadProject(IVsHierarchy pRealHierarchy, IVsHierarchy pStubHierarchy)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryCloseProject(IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryCloseSolution(object pUnkReserved, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryUnloadProject(IVsHierarchy pRealHierarchy, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }
    }
}
