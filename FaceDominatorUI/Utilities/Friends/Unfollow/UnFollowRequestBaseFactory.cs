using DominatorHouseCore.Enums;
using FaceDominatorCore.FDFactories;
using FaceDominatorCore.Interface;
using FaceDominatorCore.Utility;

namespace FaceDominatorUI.Utilities.Friends.Unfollow
{
    public class UnFollowRequestBaseFactory : IFdBaseFactory
    {
        public IFdUtilityFactory FdUtilityFactory()
        {
            var utilityFactory = new FdUtilityFactory();

            var builder = new FdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.Unfollow)
                .AddReportFactory(new UnFollowRequestReports())
                .AddViewCampaignFactory(new UnFollowRequestViewCampaign());

            return builder.FdUtilityFactory;
        }
    }
}
