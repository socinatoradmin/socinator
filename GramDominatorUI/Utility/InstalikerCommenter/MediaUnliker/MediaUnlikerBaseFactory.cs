using DominatorHouseCore.Enums;
using GramDominatorCore.GDFactories;
using GramDominatorCore.Interface;
using GramDominatorCore.Utility;

namespace GramDominatorUI.Utility.InstalikerCommenter.MediaUnliker
{
    internal class MediaUnlikerBaseFactory : IGdBaseFactory
    {
        public IGdUtilityFactory GdUtilityFactory()
        {
            var utilityFactory = new GdUtilityFactory();

            var builder = new GdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.Unlike)
                .AddReportFactory(new MediaUnlikerReports())
                .AddViewCampaignFactory(new MediaUnlikerViewCampaign());

            return builder.GdUtilityFactory;
        }
    }
}