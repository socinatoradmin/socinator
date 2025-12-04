using DominatorHouseCore.Enums;
using GramDominatorCore.GDFactories;
using GramDominatorCore.Interface;
using GramDominatorCore.Utility;

namespace GramDominatorUI.Utility.InstalikerCommenter.Comment
{
    internal class CommentBaseFactory : IGdBaseFactory
    {
        public IGdUtilityFactory GdUtilityFactory()
        {
            var utilityFactory = new GdUtilityFactory();

            var builder = new GdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.Comment)
                .AddReportFactory(new CommentReports())
                .AddViewCampaignFactory(new CommentViewCampaign());

            return builder.GdUtilityFactory;
        }
    }
}