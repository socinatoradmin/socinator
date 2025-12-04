using DominatorHouseCore.Enums;
using GramDominatorCore.GDFactories;
using GramDominatorCore.Interface;
using GramDominatorCore.Utility;

namespace GramDominatorUI.Utility.GrowFollowers.MakeCloseFriend
{
    public class CloseFriendBaseFactory: IGdBaseFactory
    {
        public IGdUtilityFactory GdUtilityFactory()
        {
            var utilityFactory = new GdUtilityFactory();

            var builder = new GdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.CloseFriend)
                .AddReportFactory(new CloseFriendReport())
                .AddViewCampaignFactory(new CloseFriendViewCampaign());

            return builder.GdUtilityFactory;
        }
    }
}
