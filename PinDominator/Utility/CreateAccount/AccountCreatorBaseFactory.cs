using DominatorHouseCore.Enums;
using PinDominatorCore.Interface;
using PinDominatorCore.PDFactories;
using PinDominatorCore.Utility;

namespace PinDominator.Utility.CreateAccount
{
    public class AccountCreatorBaseFactory : IPdBaseFactory
    {
        public IPdUtilityFactory PdUtilityFactory()
        {
            var utilityFactory = new PdUtilityFactory();
            var builder = new PdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.CreateAccount)
                .AddReportFactory(new AccountCreatorReports())
                .AddViewCampaignFactory(new AccountCreatorViewCampaign());

            return builder.PdUtilityFactory;
        }
    }
}