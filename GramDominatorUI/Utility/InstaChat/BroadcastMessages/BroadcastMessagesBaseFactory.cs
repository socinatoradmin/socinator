using DominatorHouseCore.Enums;
using GramDominatorCore.GDFactories;
using GramDominatorCore.Interface;
using GramDominatorCore.Utility;

namespace GramDominatorUI.Utility.InstaChat.BroadcastMessages
{
    internal class BroadcastMessagesBaseFactory : IGdBaseFactory
    {
        public IGdUtilityFactory GdUtilityFactory()
        {
            var utilityFactory = new GdUtilityFactory();

            var builder = new GdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.BroadcastMessages)
                .AddReportFactory(new BroadcastMessagesReports())
                .AddViewCampaignFactory(new BroadcastMessagesViewCampaign());

            return builder.GdUtilityFactory;
        }
    }
}