using DominatorHouseCore.BusinessLogic.Scraper;
using DominatorHouseCore.Process;
using TumblrDominatorCore.TumblrLibrary.TumblrProcesses;
using Unity;

namespace TumblrDominatorCore.TumblrFactory
{
    public interface ITumblrQueryScraperFactory : IQueryScraperFactory
    {
    }

    public class TumblrQueryScraperFactory : ITumblrQueryScraperFactory
    {
        private readonly IUnityContainer _unityContainer;

        public TumblrQueryScraperFactory(IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
        }

        /// <summary>
        ///     Assigns JobProcess objects to tumblrjobprocess object
        ///     and initializes objects
        /// </summary>
        /// <param name="jobProcess"></param>
        /// <returns></returns>
        public QueryScraper Create(JobProcess jobProcess)
        {
            var tumblrScraper = _unityContainer.Resolve<ITumblrScraperActionTables>();

            return new TumblrQueryScraper((TumblrJobProcess)jobProcess, tumblrScraper.ScrapeWithQueriesActionTable,
                tumblrScraper.ScrapeWithoutQueriesActionTable);
        }
    }
}