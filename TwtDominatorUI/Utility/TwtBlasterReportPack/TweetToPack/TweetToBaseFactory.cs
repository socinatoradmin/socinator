using DominatorHouseCore.Enums;
using TwtDominatorCore.Interface;
using TwtDominatorCore.TDFactories;
using TwtDominatorCore.TDUtility;

namespace TwtDominatorUI.Utility.TwtBlasterReportPack.TweetToPack
{
    public class TweetToBaseFactory : ITDBaseFactory
    {
        public ITDUtilityFactory TDUtilityFactory()
        {
            var utilityFactory = new TDUtilityFactory();

            var builder = new TDBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.TweetTo)
                .AddReportFactory(new TweetToReport())
                .AddViewCampaignFactory(new TweetToViewCampaign());

            return builder.TDUtilityFactory;
        }
    }
}