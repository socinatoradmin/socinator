using DominatorHouseCore.Enums;
using RedditDominatorCore.RDFactories;
using RedditDominatorCore.Utility;

namespace RedditDominatorUI.Utility.AutoActivity
{
    public class AutoActivityBaseFactory : IRdBaseFactory
    {
        public IRdUtilityFactory RdUtilityFactory()
        {
            var utilityFactory = new RdUtilityFactory();

            var builder = new RdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.AutoActivity)
                .AddReportFactory(new AutoActivityReport())
                .AddViewCampaignFactory(new AutoActivityViewCampaign());
            return builder.RdUtilityFactory;
        }
    }
}
