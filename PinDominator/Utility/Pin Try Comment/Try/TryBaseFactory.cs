using DominatorHouseCore.Enums;
using PinDominatorCore.Interface;
using PinDominatorCore.PDFactories;
using PinDominatorCore.Utility;

namespace PinDominator.Utility.Pin_Try_Comment.Try
{
    public class TryBaseFactory : IPdBaseFactory
    {
        public IPdUtilityFactory PdUtilityFactory()
        {
            var utilityFactory = new PdUtilityFactory();

            var builder = new PdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.Try)
                .AddReportFactory(new TryReports())
                .AddViewCampaignFactory(new TryViewCampaigns());

            return builder.PdUtilityFactory;
        }
    }
}