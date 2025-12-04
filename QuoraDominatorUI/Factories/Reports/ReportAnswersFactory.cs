using DominatorHouseCore.Enums;
using QuoraDominatorCore.Interface;
using QuoraDominatorCore.QDFactories;
using QuoraDominatorCore.QdUtility;
using QuoraDominatorUI.Utility.Report.Reports;
using QuoraDominatorUI.Utility.ViewCampaign.Reports;

namespace QuoraDominatorUI.Factories.Reports
{
    public class ReportAnswersFactory : IQdBaseFactory
    {
        public IQdUtilityFactory QdUtilityFactory()
        {
            var utilityFactory = new QdUtilityFactory();

            var builder = new QdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.ReportAnswers)
                .AddReportFactory(new ReportAnswersReports())
                .AddViewCampaignFactory(new ReportAnswersViewCampaign());

            return builder.QdUtilityFactory;
        }
    }
}