using DominatorHouseCore.Enums;
using TwtDominatorCore.Interface;
using TwtDominatorCore.TDFactories;
using TwtDominatorCore.TDUtility;

namespace TwtDominatorUI.Utility.TwtBlasterReportPack.RetweetPack
{
    public class RetweetBaseFactory : ITDBaseFactory
    {
        public ITDUtilityFactory TDUtilityFactory()
        {
            var utilityFactory = new TDUtilityFactory();

            var builder = new TDBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.Retweet)
                .AddReportFactory(new RetweetReport())
                .AddViewCampaignFactory(new RetweetViewCampaign());

            return builder.TDUtilityFactory;
        }
    }
}