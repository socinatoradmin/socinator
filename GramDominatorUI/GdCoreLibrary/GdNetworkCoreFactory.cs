using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.BusinessLogic.Scraper;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;

namespace GramDominatorUI.GDCoreLibrary
{
    public class GdNetworkCoreFactory : INetworkCoreFactory
    {
        /// <summary>
        ///     creates job process based on social network and module
        /// </summary>
        public IJobProcessFactory JobProcessFactory { get; set; }

        /// <summary>
        ///     Scraps data from social network feed based on query (queries)
        /// </summary>
        public IQueryScraperFactory QueryScraperFactory { get; set; }

        public ICampaignInteractionDetails CampaignInteractionDetails { get; set; }

        public IGlobalInteractionDetails GlobalInteractionDetails { get; set; }

        public IDatabaseConnection AccountCreateDatabase { get; set; }

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
    }
}