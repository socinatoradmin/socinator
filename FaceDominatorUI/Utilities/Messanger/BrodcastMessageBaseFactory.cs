using DominatorHouseCore.Enums;
using FaceDominatorCore.FDFactories;
using FaceDominatorCore.Interface;
using FaceDominatorCore.Utility;

namespace FaceDominatorUI.Utilities.Messanger
{
    public class BrodcastMessageBaseFactory : IFdBaseFactory
    {
        public IFdUtilityFactory FdUtilityFactory()
        {
            var utilityFactory = new FdUtilityFactory();

            var builder = new FdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.BroadcastMessages)
                .AddReportFactory(new BrodcastMessageReports())
                .AddViewCampaignFactory(new BrodcastMessageViewCampaign());

            return builder.FdUtilityFactory;
        }
    }
}