#region

using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;

#endregion

namespace DominatorHouseCore.Diagnostics
{
    public class NetworkCoreLibraryBuilder
    {
        public NetworkCoreLibraryBuilder(INetworkCoreFactory networkCoreFactory)
        {
            NetworkCoreFactory = networkCoreFactory;
        }

        public INetworkCoreFactory NetworkCoreFactory { get; set; }

        public NetworkCoreLibraryBuilder AddNetwork(SocialNetworks networks)
        {
            NetworkCoreFactory.Network = networks;
            return this;
        }

        public NetworkCoreLibraryBuilder AddTabFactory(ITabHandlerFactory tabFactory)
        {
            NetworkCoreFactory.TabHandlerFactory = tabFactory;
            return this;
        }

        public NetworkCoreLibraryBuilder AddAccountFactory(IAccountUpdateFactory accountUpdate)
        {
            NetworkCoreFactory.AccountUpdateFactory = accountUpdate;
            return this;
        }

        public NetworkCoreLibraryBuilder AddAccountCounts(IAccountCountFactory accountCount)
        {
            NetworkCoreFactory.AccountCountFactory = accountCount;
            return this;
        }

        public NetworkCoreLibraryBuilder AddAccountUiTools(IAccountToolsFactory accountUserControl)
        {
            NetworkCoreFactory.AccountUserControlTools = accountUserControl;
            return this;
        }


        public NetworkCoreLibraryBuilder AddAccountSelectors(IDestinationSelectors destinationSelectors)
        {
            NetworkCoreFactory.AccountDetailsSelectors = destinationSelectors;
            return this;
        }

        public NetworkCoreLibraryBuilder AddAccountDbConnection(IDatabaseConnection accountDbConnection)
        {
            NetworkCoreFactory.AccountDatabase = accountDbConnection;
            return this;
        }

        public NetworkCoreLibraryBuilder AddCampaignDbConnection(IDatabaseConnection campaignDbConnection)
        {
            NetworkCoreFactory.CampaignDatabase = campaignDbConnection;
            return this;
        }

        public NetworkCoreLibraryBuilder AddReportFactory(IReportFactory reportFactory)
        {
            NetworkCoreFactory.ReportFactory = reportFactory;
            return this;
        }

        public NetworkCoreLibraryBuilder AddViewCampaignFactory(IViewCampaignsFactory viewCampaigns)
        {
            NetworkCoreFactory.ViewCampaigns = viewCampaigns;
            return this;
        }

        public NetworkCoreLibraryBuilder AddAccountVerificationFactory(IAccountVerificationFactory accountVerification)
        {
            NetworkCoreFactory.AccountVerificationFactory = accountVerification;
            return this;
        }

        public NetworkCoreLibraryBuilder AddProfileFactory(ProfileFactory profileFactory)
        {
            NetworkCoreFactory.ProfileFactory = profileFactory;
            return this;
        }

        public NetworkCoreLibraryBuilder AddChatFactory(ChatFactory chatFactory)
        {
            NetworkCoreFactory.ChatFactory = chatFactory;
            return this;
        }

        //public NetworkCoreLibraryBuilder AddAdScraperFactory(IAdScraperFactory adScraperUpdate)
        //{
        //    NetworkCoreFactory.AdScraperFactory = adScraperUpdate;
        //    return this;
        //}
    }
}