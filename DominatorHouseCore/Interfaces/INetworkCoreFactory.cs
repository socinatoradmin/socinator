#region

using DominatorHouseCore.Enums;

#endregion

namespace DominatorHouseCore.Interfaces
{
    public interface INetworkCoreFactory
    {
        /// <summary>
        ///     Specify the network of the dominator
        /// </summary>
        SocialNetworks Network { get; set; }

        ITabHandlerFactory TabHandlerFactory { get; set; }

        IAccountUpdateFactory AccountUpdateFactory { get; set; }

        IAccountCountFactory AccountCountFactory { get; set; }

        IAccountToolsFactory AccountUserControlTools { get; set; }


        IDestinationSelectors AccountDetailsSelectors { get; set; }

        IDatabaseConnection AccountDatabase { get; set; }

        IDatabaseConnection CampaignDatabase { get; set; }

        IReportFactory ReportFactory { get; set; }

        IViewCampaignsFactory ViewCampaigns { get; set; }

        IAccountVerificationFactory AccountVerificationFactory { get; set; }
        ProfileFactory ProfileFactory { get; set; }
        ChatFactory ChatFactory { get; set; }

        //IAdScraperFactory AdScraperFactory { get; set; }
    }
}