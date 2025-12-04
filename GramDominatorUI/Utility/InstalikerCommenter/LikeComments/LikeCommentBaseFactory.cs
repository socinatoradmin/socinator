using DominatorHouseCore.Enums;
using GramDominatorCore.GDFactories;
using GramDominatorCore.Interface;
using GramDominatorCore.Utility;

namespace GramDominatorUI.Utility.InstalikerCommenter.LikeComments
{
    public class LikeCommentBaseFactory : IGdBaseFactory
    {
        public IGdUtilityFactory GdUtilityFactory()
        {
            var utilityFactory = new GdUtilityFactory();

            var builder = new GdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.LikeComment)
                .AddReportFactory(new LikeCommentReports())
                .AddViewCampaignFactory(new LikeCommentViewCampaign());

            return builder.GdUtilityFactory;
        }
    }
}