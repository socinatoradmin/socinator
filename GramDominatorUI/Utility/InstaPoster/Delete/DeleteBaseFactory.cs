using DominatorHouseCore.Enums;
using GramDominatorCore.GDFactories;
using GramDominatorCore.Interface;
using GramDominatorCore.Utility;

namespace GramDominatorUI.Utility.InstaPoster.Delete
{
    internal class DeleteBaseFactory : IGdBaseFactory
    {
        public IGdUtilityFactory GdUtilityFactory()
        {
            var utilityFactory = new GdUtilityFactory();

            var builder = new GdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.DeletePost)
                .AddReportFactory(new DeleteReports())
                .AddViewCampaignFactory(new DeleteViewCampaign());

            return builder.GdUtilityFactory;
        }
    }
}