using DominatorHouseCore.Enums;
using PinDominatorCore.Interface;
using PinDominatorCore.PDFactories;
using PinDominatorCore.Utility;

namespace PinDominator.Utility.PinPoster.DeletePin
{
    public class DeletePinBaseFactory : IPdBaseFactory
    {
        public IPdUtilityFactory PdUtilityFactory()
        {
            var utilityFactory = new PdUtilityFactory();

            var builder = new PdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.DeletePin)
                .AddReportFactory(new DeletePinReports())
                .AddViewCampaignFactory(new DeletePinViewCampaigns());

            return builder.PdUtilityFactory;
        }
    }
}