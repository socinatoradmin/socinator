using DominatorHouseCore.Enums;
using PinDominatorCore.Interface;
using PinDominatorCore.PDFactories;
using PinDominatorCore.Utility;

namespace PinDominator.Utility.PinPoster.RePin
{
    public class RePinBaseFactory : IPdBaseFactory
    {
        public IPdUtilityFactory PdUtilityFactory()
        {
            var utilityFactory = new PdUtilityFactory();

            var builder = new PdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.Repin)
                .AddReportFactory(new RePinReports())
                .AddViewCampaignFactory(new RePinViewCampaigns());

            return builder.PdUtilityFactory;
        }
    }
}