// <copyright file="MyHistoryPage.cs" company="Microsoft Corporation">Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.</copyright>
namespace Microsoft.ALMRangers.Samples.MyHistory
{
    using Microsoft.TeamFoundation.Controls;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
    using System;
    using System.Reflection;

    /// <summary>
    /// MyHistory Page. We are extending Team Explorer by adding a new page and therefore use the TeamExplorerPage attribute and pass in our unique ID
    /// </summary>
    [TeamExplorerPage(MyTimePage.PageId)]
    [ProvideAutoLoad(UIContextGuids80.SolutionExists)]
    public class MyTimePage : TeamExplorerBasePage
    {
        // All Pages must have a unique ID. Use the Tools - Create GUID menu in Visual Studio to create your own GUID
        public const string PageId = "D55F1163-26B2-4C70-A715-1D6285ABDC20";
                
        public MyTimePage()
        {
            // Set the page title
            this.Title = "My Time";
        }
    }

    public static class EmbeddedAssemblyResolver
    {
        static EmbeddedAssemblyResolver()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                String resourceName = "Company.Timekeeper." +

                   new AssemblyName(args.Name).Name + ".dll";
                var names = typeof(MyTimePage).Assembly.GetManifestResourceNames();
                using (var stream = typeof(MyTimePage).Assembly.GetManifestResourceStream(resourceName))
                {
                    Byte[] assemblyData = new Byte[stream.Length];

                    stream.Read(assemblyData, 0, assemblyData.Length);

                    return Assembly.Load(assemblyData);
                }
            };
        }

        public static void Ensure() { }
    }
}