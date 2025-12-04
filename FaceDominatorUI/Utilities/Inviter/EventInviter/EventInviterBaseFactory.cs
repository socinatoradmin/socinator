using DominatorHouseCore.Enums;
using FaceDominatorCore.FDFactories;
using FaceDominatorCore.Interface;
using FaceDominatorCore.Utility;

namespace FaceDominatorUI.Utilities.Inviter.EventInviter
{
    public class EventInviterBaseFactory : IFdBaseFactory
    {
        public IFdUtilityFactory FdUtilityFactory()
        {
            var utilityFactory = new FdUtilityFactory();

            var builder = new FdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.EventInviter)
                .AddReportFactory(new EventInviterReports())
                .AddViewCampaignFactory(new EventInviterViewCampaign());

            return builder.FdUtilityFactory;
        }
    }
}