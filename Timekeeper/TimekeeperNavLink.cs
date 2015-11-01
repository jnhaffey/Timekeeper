using System;
using System.ComponentModel.Composition;
using Microsoft.TeamFoundation.Controls;
using Microsoft.VisualStudio.Shell;
using Timekeeper.VsExtension;

namespace Timekeeper.VsExtension
{
    [TeamExplorerNavigationLink(TimekeeperNavigationLink.LinkId, TeamExplorerNavigationItemIds.MyWork, 200)]
    public class TimekeeperNavigationLink : TeamExplorerBaseNavigationLink
    {
        public const string LinkId = "82B2A067-0720-48D3-AD34-E220AD8DBBE1";

        [ImportingConstructor]
        public TimekeeperNavigationLink([Import(typeof(SVsServiceProvider))] IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            this.Text = "My Time";
            this.IsVisible = true;
            this.IsEnabled = true;
        }

        public override void Execute()
        {
            try
            {
                ITeamExplorer teamExplorer = GetService<ITeamExplorer>();
                if (teamExplorer != null)
                {
                    teamExplorer.NavigateToPage(new Guid(MyTimePage.PageId), null);
                }
            }
            catch (Exception ex)
            {
                this.ShowNotification(ex.Message, NotificationType.Error);
            }
        }

        public override void Invalidate()
        {
            base.Invalidate();
            this.IsEnabled = true;
            this.IsVisible = true;
        }
    }
}
