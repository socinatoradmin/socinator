using DominatorHouseCore.Enums;
using QuoraDominatorCore.Interface;
using QuoraDominatorCore.QDFactories;
using QuoraDominatorCore.QdUtility;
using QuoraDominatorUI.Utility.Report.Answer;
using QuoraDominatorUI.Utility.ViewCampaign.GrowFollower;

namespace QuoraDominatorUI.Factories.GrowFollower
{
    public class AnswerOnQuestionFactory : IQdBaseFactory
    {
        public IQdUtilityFactory QdUtilityFactory()
        {
            var utilityFactory = new QdUtilityFactory();

            var builder = new QdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.AnswerOnQuestions)
                .AddReportFactory(new AnswerOnQuestionReports())
                .AddViewCampaignFactory(new AnswerOnQuestionViewCampaign());

            return builder.QdUtilityFactory;
        }
    }
}