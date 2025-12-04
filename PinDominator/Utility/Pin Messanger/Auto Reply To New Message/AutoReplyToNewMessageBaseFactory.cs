using DominatorHouseCore.Enums;
using PinDominatorCore.Interface;
using PinDominatorCore.PDFactories;
using PinDominatorCore.Utility;

namespace PinDominator.Utility.Pin_Messanger.Auto_Reply_To_New_Message
{
    public class AutoReplyToNewMessageBaseFactory : IPdBaseFactory
    {
        public IPdUtilityFactory PdUtilityFactory()
        {
            var utilityFactory = new PdUtilityFactory();

            var builder = new PdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.AutoReplyToNewMessage)
                .AddReportFactory(new AutoReplyToNewMessageReports())
                .AddViewCampaignFactory(new AutoReplyToNewMessageViewCampaign());

            return builder.PdUtilityFactory;
        }
    }
}