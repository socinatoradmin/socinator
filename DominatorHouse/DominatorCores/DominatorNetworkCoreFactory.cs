using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;

namespace Socinator.DominatorCores
{
    internal class DominatorNetworkCoreFactory : INetworkCoreFactory
    {

        /// <summary>
        ///     Specify the network of the dominator
        /// </summary>
        public SocialNetworks Network { get; set; }

        public ITabHandlerFactory TabHandlerFactory { get; set; }

        public IAccountUpdateFactory AccountUpdateFactory { get; set; }

        public IAccountCountFactory AccountCountFactory { get; set; }

        public IAccountToolsFactory AccountUserControlTools { get; set; }

        public IDestinationSelectors AccountDetailsSelectors { get; set; }

        public IDatabaseConnection AccountDatabase { get; set; }

        public IDatabaseConnection CampaignDatabase { get; set; }

        public IReportFactory ReportFactory { get; set; }

        public IViewCampaignsFactory ViewCampaigns { get; set; }

        public IAccountVerificationFactory AccountVerificationFactory { get; set; }
        public ProfileFactory ProfileFactory { get; set; }
        public ChatFactory ChatFactory { get; set; }
        public IGlobalInteractionDetails GlobalInteractionDetails { get; set; }
        public IAdScraperFactory AdScraperFactory { get; set; }
    }
}