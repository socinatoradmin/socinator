using DominatorHouseCore.Enums;
using TwtDominatorCore.Interface;
using TwtDominatorCore.TDFactories;
using TwtDominatorCore.TDUtility;

namespace TwtDominatorUI.Utility.TwtEngageReportPack.CommentPack
{
    public class CommentBaseFactory : ITDBaseFactory
    {
        public ITDUtilityFactory TDUtilityFactory()
        {
            var utilityFactory = new TDUtilityFactory();

            var builder = new TDBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.Comment)
                .AddReportFactory(new CommentReport())
                .AddViewCampaignFactory(new CommentViewCampaign());

            return builder.TDUtilityFactory;
        }
    }
}