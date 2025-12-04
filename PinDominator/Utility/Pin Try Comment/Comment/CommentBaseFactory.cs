using DominatorHouseCore.Enums;
using PinDominatorCore.Interface;
using PinDominatorCore.PDFactories;
using PinDominatorCore.Utility;

namespace PinDominator.Utility.Pin_Try_Comment.Comment
{
    public class CommentBaseFactory : IPdBaseFactory
    {
        public IPdUtilityFactory PdUtilityFactory()
        {
            var utilityFactory = new PdUtilityFactory();

            var builder = new PdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.Comment)
                .AddReportFactory(new CommentReports())
                .AddViewCampaignFactory(new CommentViewCampaigns());

            return builder.PdUtilityFactory;
        }
    }
}