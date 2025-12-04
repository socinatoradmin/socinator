using DominatorHouseCore.Enums;
using QuoraDominatorCore.Interface;
using QuoraDominatorCore.QDFactories;
using QuoraDominatorCore.QdUtility;
using QuoraDominatorUI.Utility.Report.Reports;
using QuoraDominatorUI.Utility.ViewCampaign.Reports;

namespace QuoraDominatorUI.Factories.Reports
{
    public class ReportUsersFactory : IQdBaseFactory
    {
        public IQdUtilityFactory QdUtilityFactory()
        {
            var utilityFactory = new QdUtilityFactory();

            var builder = new QdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.ReportUsers)
                .AddReportFactory(new ReportUsersReports())
                .AddViewCampaignFactory(new ReportUsersViewCampaign());

            return builder.QdUtilityFactory;
        }
    }
}