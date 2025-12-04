using DominatorHouseCore.Enums;
using QuoraDominatorCore.Interface;
using QuoraDominatorCore.QDFactories;
using QuoraDominatorCore.QdUtility;
using QuoraDominatorUI.Utility.Report.Scrape;
using QuoraDominatorUI.Utility.ViewCampaign.Scrape;

namespace QuoraDominatorUI.Factories.Scrape
{
    public class QuestionsScraperFactory : IQdBaseFactory
    {
        public IQdUtilityFactory QdUtilityFactory()
        {
            var utilityFactory = new QdUtilityFactory();

            var builder = new QdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.QuestionsScraper)
                .AddReportFactory(new QuestionsScraperReports())
                .AddViewCampaignFactory(new QuestionsScraperViewCampaign());

            return builder.QdUtilityFactory;
        }
    }
}