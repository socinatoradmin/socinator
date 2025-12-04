using DominatorHouseCore.Enums;
using GramDominatorCore.GDFactories;
using GramDominatorCore.Interface;
using GramDominatorCore.Utility;

namespace GramDominatorUI.Utility.InstalikerCommenter.Like
{
    internal class LikeBaseFactory : IGdBaseFactory
    {
        public IGdUtilityFactory GdUtilityFactory()
        {
            var utilityFactory = new GdUtilityFactory();

            var builder = new GdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.Like)
                .AddReportFactory(new LikeReports())
                .AddViewCampaignFactory(new LikeViewCampaign());

            return builder.GdUtilityFactory;
        }
    }
}