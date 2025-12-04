using DominatorHouseCore.Enums;
using FaceDominatorCore.FDFactories;
using FaceDominatorCore.Interface;
using FaceDominatorCore.Utility;

namespace FaceDominatorUI.Utilities.Inviter.GroupInviter
{
    public class GroupInviterBaseFactory : IFdBaseFactory
    {
        public IFdUtilityFactory FdUtilityFactory()
        {
            var utilityFactory = new FdUtilityFactory();

            var builder = new FdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.FanpageLiker)
                .AddReportFactory(new GroupInviterReports())
                .AddViewCampaignFactory(new GroupInviterViewCampaign());

            return builder.FdUtilityFactory;
        }
    }
}