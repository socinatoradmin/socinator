using DominatorHouseCore.Enums;
using FaceDominatorCore.FDFactories;
using FaceDominatorCore.Interface;
using FaceDominatorCore.Utility;

namespace FaceDominatorUI.Utilities.Messanger.AutoReplyMessage
{
    public class AutoReplyMessageBaseFactory : IFdBaseFactory
    {
        public IFdUtilityFactory FdUtilityFactory()
        {
            var utilityFactory = new FdUtilityFactory();

            var builder = new FdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.AutoReplyToNewMessage)
                .AddReportFactory(new AutoReplyMessageReports())
                .AddViewCampaignFactory(new AutoReplyMessageViewCampaign());

            return builder.FdUtilityFactory;
        }
    }
}