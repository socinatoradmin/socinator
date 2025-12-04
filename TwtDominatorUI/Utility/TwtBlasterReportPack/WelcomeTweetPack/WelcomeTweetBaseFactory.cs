using DominatorHouseCore.Enums;
using TwtDominatorCore.Interface;
using TwtDominatorCore.TDFactories;
using TwtDominatorCore.TDUtility;

namespace TwtDominatorUI.Utility.TwtBlasterReportPack.WelcomeTweetPack
{
    public class WelcomeTweetBaseFactory : ITDBaseFactory
    {
        public ITDUtilityFactory TDUtilityFactory()
        {
            var utilityFactory = new TDUtilityFactory();

            var builder = new TDBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.WelcomeTweet)
                .AddReportFactory(new WelcomeTweetReport())
                .AddViewCampaignFactory(new WelcomeTweetViewCampaign());

            return builder.TDUtilityFactory;
        }
    }
}