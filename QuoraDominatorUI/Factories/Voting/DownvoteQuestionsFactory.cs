using DominatorHouseCore.Enums;
using QuoraDominatorCore.Interface;
using QuoraDominatorCore.QDFactories;
using QuoraDominatorCore.QdUtility;
using QuoraDominatorUI.Utility.Report.Voting;
using QuoraDominatorUI.Utility.ViewCampaign.Voting;

namespace QuoraDominatorUI.Factories.Voting
{
    public class DownvoteQuestionsFactory : IQdBaseFactory
    {
        public IQdUtilityFactory QdUtilityFactory()
        {
            var utilityFactory = new QdUtilityFactory();

            var builder = new QdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.DownvoteQuestions)
                .AddReportFactory(new DownvoteQuestionsReports())
                .AddViewCampaignFactory(new DownvoteQuestionsViewCampaign());

            return builder.QdUtilityFactory;
        }
    }
}