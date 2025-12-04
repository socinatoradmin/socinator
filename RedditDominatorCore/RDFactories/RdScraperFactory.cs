using DominatorHouseCore.BusinessLogic.Scraper;
using DominatorHouseCore.Process;
using RedditDominatorCore.RDLibrary;
using Unity;

namespace RedditDominatorCore.RDFactories
{
    public interface IRdQueryScraperFactory : IQueryScraperFactory
    {
    }

    public class RdQueryScraperFactory : IRdQueryScraperFactory
    {
        private readonly IUnityContainer _unityContainer;

        public RdQueryScraperFactory(IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
        }

        public QueryScraper Create(JobProcess jobProcess)
        {
            var redditScraper = _unityContainer.Resolve<IRedditScraperActionTables>();
            return new RedditQueryScraper((IRdJobProcess)jobProcess, redditScraper.ScrapeWithQueriesActionTable,
                redditScraper.ScrapeWithoutQueriesActionTable);
        }
    }
}