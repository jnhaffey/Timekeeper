// <copyright file="MyHistoryPackage.cs" company="Microsoft Corporation">Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.</copyright>
namespace Microsoft.ALMRangers.Samples.MyHistory
{
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;
    using Microsoft.TeamFoundation.Client;
    using Microsoft.TeamFoundation.VersionControl.Client;
    using System;
    using Microsoft.VisualStudio.TeamFoundation;
    using Microsoft.VisualStudio;

    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// </summary>
    [PackageRegistration(UseManagedResourcesOnly = true)]     // This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is a package.
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]     // This attribute is used to register the information needed to show this package in the Help/About dialog of Visual Studio.
    [Guid("e49a882b-1677-46a9-93b4-db290943bbcd")]
    [ProvideAutoLoad(VSConstants.UICONTEXT.NoSolution_string)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionExists_string)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionHasMultipleProjects_string)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionHasSingleProject_string)]
    public sealed class MyHistoryPackage : Package
    {
        protected override void Initialize()
        {
            base.Initialize();

            var dte = Package.GetGlobalService(typeof(EnvDTE.DTE)) as EnvDTE.DTE;
            TeamFoundationServerExt ext = dte.GetObject("Microsoft.VisualStudio.TeamFoundation.TeamFoundationServerExt") as TeamFoundationServerExt;
            TfsTeamProjectCollection tfs = new TfsTeamProjectCollection(new Uri(ext.ActiveProjectContext.DomainUri));
            VersionControlServer vcs = tfs.GetService<VersionControlServer>();
            vcs.UnshelveShelveset += vcs_UnshelveShelveset;
            vcs.ShelvesetUpdated += vcs_ShelvesetUpdated;
            vcs.CommitShelveset += vcs_CommitShelveset;
        }

        void vcs_CommitShelveset(object sender, CommitShelvesetEventArgs e)
        {
            var shelf = e.Shelveset;
        }

        void vcs_ShelvesetUpdated(object sender, ShelvesetUpdatedEventArgs e)
        {
            var shelf = e.Shelveset;
        }

        void vcs_UnshelveShelveset(object sender, UnshelveShelvesetEventArgs e)
        {
            var shelf = e.Shelveset;
        }
    }
}
