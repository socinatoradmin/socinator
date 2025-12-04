using DominatorHouseCore.Enums;
using TwtDominatorCore.Interface;
using TwtDominatorCore.TDFactories;
using TwtDominatorCore.TDUtility;

namespace TwtDominatorUI.Utility.GrowFollowersReportPack.MuteUsersPack
{
    public class MuteUsersBaseFactory : ITDBaseFactory
    {
        public ITDUtilityFactory TDUtilityFactory()
        {
            var utilityFactory = new TDUtilityFactory();

            var builder = new TDBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.Mute)
                .AddReportFactory(new MuteUsersReport())
                .AddViewCampaignFactory(new MuteUsersViewCampaign());

            return builder.TDUtilityFactory;
        }
    }
}