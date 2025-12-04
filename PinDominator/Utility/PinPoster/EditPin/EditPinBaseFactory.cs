using DominatorHouseCore.Enums;
using PinDominatorCore.Interface;
using PinDominatorCore.PDFactories;
using PinDominatorCore.Utility;

namespace PinDominator.Utility.PinPoster.EditPin
{
    public class EditPinBaseFactory : IPdBaseFactory
    {
        public IPdUtilityFactory PdUtilityFactory()
        {
            var utilityFactory = new PdUtilityFactory();

            var builder = new PdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.EditPin)
                .AddReportFactory(new EditPinReports())
                .AddViewCampaignFactory(new EditPinViewCampaigns());

            return builder.PdUtilityFactory;
        }
    }
}