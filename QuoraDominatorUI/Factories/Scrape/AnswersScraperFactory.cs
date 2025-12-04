using DominatorHouseCore.Enums;
using QuoraDominatorCore.Interface;
using QuoraDominatorCore.QDFactories;
using QuoraDominatorCore.QdUtility;
using QuoraDominatorUI.Utility.Report.Scrape;
using QuoraDominatorUI.Utility.ViewCampaign.Scrape;

namespace QuoraDominatorUI.Factories.Scrape
{
    public class AnswersScraperFactory : IQdBaseFactory
    {
        public IQdUtilityFactory QdUtilityFactory()
        {
            var utilityFactory = new QdUtilityFactory();

            var builder = new QdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.AnswersScraper)
                .AddReportFactory(new AnswersScraperReports())
                .AddViewCampaignFactory(new AnswersScraperViewCampaign());

            return builder.QdUtilityFactory;
        }
    }
}