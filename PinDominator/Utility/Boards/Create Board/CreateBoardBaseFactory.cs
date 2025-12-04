using DominatorHouseCore.Enums;
using PinDominatorCore.Interface;
using PinDominatorCore.PDFactories;
using PinDominatorCore.Utility;

namespace PinDominator.Utility.Boards.Create_Board
{
    public class CreateBoardBaseFactory : IPdBaseFactory
    {
        public IPdUtilityFactory PdUtilityFactory()
        {
            var utilityFactory = new PdUtilityFactory();

            var builder = new PdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.CreateBoard)
                .AddReportFactory(new CreateBoardReports())
                .AddViewCampaignFactory(new CreateBoardViewCampaign());

            return builder.PdUtilityFactory;
        }
    }
}