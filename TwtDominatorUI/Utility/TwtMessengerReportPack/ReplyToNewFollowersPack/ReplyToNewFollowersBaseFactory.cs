using DominatorHouseCore.Enums;
using TwtDominatorCore.Interface;
using TwtDominatorCore.TDFactories;
using TwtDominatorCore.TDUtility;
using TwtDominatorUI.Utility.TwtEngageReportPack.ReplyToNewFollowersPack;

namespace TwtDominatorUI.Utility.TwtMessengerReportPack.ReplyToNewFollowersPack
{
    public class ReplyToNewFollowersBaseFactory : ITDBaseFactory
    {
        public ITDUtilityFactory TDUtilityFactory()
        {
            var utilityFactory = new TDUtilityFactory();

            var builder = new TDBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.SendMessageToFollower)
                .AddReportFactory(new ReplyToNewFollowersReport())
                .AddViewCampaignFactory(new ReplyToNewFollowersViewCampaign());

            return builder.TDUtilityFactory;
        }
    }
}