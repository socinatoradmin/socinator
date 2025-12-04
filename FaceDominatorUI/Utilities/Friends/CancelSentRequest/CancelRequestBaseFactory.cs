using DominatorHouseCore.Enums;
using FaceDominatorCore.FDFactories;
using FaceDominatorCore.Interface;
using FaceDominatorCore.Utility;

namespace FaceDominatorUI.Utilities.Friends.CancelSentRequest
{
    public class CancelRequestBaseFactory : IFdBaseFactory
    {
        public IFdUtilityFactory FdUtilityFactory()
        {
            var utilityFactory = new FdUtilityFactory();

            var builder = new FdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.WithdrawSentRequest)
                .AddReportFactory(new CancelRequestReports())
                .AddViewCampaignFactory(new CancelRequestViewCampaign());

            return builder.FdUtilityFactory;
        }
    }
}