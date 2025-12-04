using DominatorHouseCore.Enums;
using FaceDominatorCore.FDFactories;
using FaceDominatorCore.Interface;
using FaceDominatorCore.Utility;

namespace FaceDominatorUI.Utilities.Inviter.PageInviter
{
    public class FanpageInviterBaseFactory : IFdBaseFactory
    {
        public IFdUtilityFactory FdUtilityFactory()
        {
            var utilityFactory = new FdUtilityFactory();

            var builder = new FdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.FanpageLiker)
                .AddReportFactory(new FanpageInviterReports())
                .AddViewCampaignFactory(new FanpageInviterViewCampaign());

            return builder.FdUtilityFactory;
        }
    }
}