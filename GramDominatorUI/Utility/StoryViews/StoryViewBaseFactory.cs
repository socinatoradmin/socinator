using DominatorHouseCore.Enums;
using GramDominatorCore.GDFactories;
using GramDominatorCore.Interface;
using GramDominatorCore.Utility;

namespace GramDominatorUI.Utility.StoryViews
{
    internal class StoryViewBaseFactory : IGdBaseFactory
    {
        public IGdUtilityFactory GdUtilityFactory()
        {
            var utilityFactory = new GdUtilityFactory();

            var builder = new GdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.StoryViewer)
                .AddReportFactory(new StoryViewReports())
                .AddViewCampaignFactory(new StoryViewCampaign());

            return builder.GdUtilityFactory;
        }
    }
}