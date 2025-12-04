using DominatorHouseCore.BusinessLogic.Scraper;
using DominatorHouseCore.Process;
using QuoraDominatorCore.QdLibrary;
using Unity;

namespace QuoraDominatorCore.Factories
{
    public interface IQdQueryScraperFactory : IQueryScraperFactory
    {
    }

    public class QdQueryScraperFactory : IQdQueryScraperFactory
    {
        private readonly IUnityContainer _unityContainer;

        public QdQueryScraperFactory(IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
        }

        public QueryScraper Create(JobProcess jobProcess)
        {
            var scraper = _unityContainer.Resolve<QuoraScraperActionTables>();

            return new QdQueryScraper((IQdJobProcess) jobProcess, scraper.ScrapeWithQueriesActionTable,
                scraper.ScrapeWithoutQueriesActionTable);
        }
    }
}