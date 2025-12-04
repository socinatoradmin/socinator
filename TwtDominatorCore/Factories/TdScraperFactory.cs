using DominatorHouseCore.BusinessLogic.Scraper;
using DominatorHouseCore.Process;
using TwtDominatorCore.TDLibrary;
using Unity;

namespace TwtDominatorCore.Factories
{
    public interface ITdQueryScraperFactory : IQueryScraperFactory
    {
    }

    public class TdQueryScraperFactory : ITdQueryScraperFactory
    {
        private readonly IUnityContainer _unityContainer;

        public TdQueryScraperFactory(IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
        }

        public QueryScraper Create(JobProcess jobProcess)
        {
            var twitterScraper = _unityContainer.Resolve<ITwitterScraperActionTables>();

            return new TwitterQueryScraper((ITdJobProcess) jobProcess, twitterScraper.ScrapeWithQueriesActionTable,
                twitterScraper.ScrapeWithoutQueriesActionTable);
        }
    }
}