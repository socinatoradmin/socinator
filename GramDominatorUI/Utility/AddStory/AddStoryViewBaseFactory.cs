using DominatorHouseCore.Enums;
using GramDominatorCore.GDFactories;
using GramDominatorCore.Interface;
using GramDominatorCore.Utility;

namespace GramDominatorUI.Utility.AddStory
{
    public class AddStoryViewBaseFactory: IGdBaseFactory
    {
        public IGdUtilityFactory GdUtilityFactory()
        {
            var utilityFactory = new GdUtilityFactory();

            var builder = new GdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.AddStory)
                .AddReportFactory(new AddStoryReport())
                .AddViewCampaignFactory(new AddStoryViewCampaign());
            return builder.GdUtilityFactory;
        }
    }
}
