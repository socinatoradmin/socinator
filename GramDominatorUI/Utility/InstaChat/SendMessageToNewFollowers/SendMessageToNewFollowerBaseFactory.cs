using DominatorHouseCore.Enums;
using GramDominatorCore.GDFactories;
using GramDominatorCore.Interface;
using GramDominatorCore.Utility;

namespace GramDominatorUI.Utility.InstaChat.SendMessageToNewFollowers
{
    internal class SendMessageToNewFollowerBaseFactory : IGdBaseFactory
    {
        public IGdUtilityFactory GdUtilityFactory()
        {
            var utilityFactory = new GdUtilityFactory();

            var builder = new GdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.SendMessageToFollower)
                .AddReportFactory(new SendMessageToNewFollowerReports())
                .AddViewCampaignFactory(new SendMessageToNewFollowerViewCampaign());

            return builder.GdUtilityFactory;
        }
    }
}