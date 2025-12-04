using DominatorHouseCore.Enums;
using FaceDominatorCore.FDFactories;
using FaceDominatorCore.Interface;
using FaceDominatorCore.Utility;

namespace FaceDominatorUI.Utilities.Inviter.WatchPartyInviter
{
    public class WatchPartyInviterBaseFactory : IFdBaseFactory
    {
        public IFdUtilityFactory FdUtilityFactory()
        {
            var utilityFactory = new FdUtilityFactory();

            var builder = new FdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.WatchPartyInviter)
                .AddReportFactory(new WatchPartyInviterReports())
                .AddViewCampaignFactory(new WatchPartyInviterViewCampaign());

            return builder.FdUtilityFactory;
        }
    }
}