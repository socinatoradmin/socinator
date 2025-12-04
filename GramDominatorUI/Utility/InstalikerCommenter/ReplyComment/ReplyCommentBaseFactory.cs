using DominatorHouseCore.Enums;
using GramDominatorCore.GDFactories;
using GramDominatorCore.Interface;
using GramDominatorCore.Utility;

namespace GramDominatorUI.Utility.InstalikerCommenter.ReplyComment
{
    internal class ReplyCommentBaseFactory : IGdBaseFactory
    {
        public IGdUtilityFactory GdUtilityFactory()
        {
            var utilityFactory = new GdUtilityFactory();

            var builder = new GdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.ReplyToComment)
                .AddReportFactory(new ReplyCommentReports())
                .AddViewCampaignFactory(new ReplyCommentsViewCampaign());

            return builder.GdUtilityFactory;
        }
    }
}