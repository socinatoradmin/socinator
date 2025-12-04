using DominatorHouseCore.BusinessLogic.Scraper;
using DominatorHouseCore.Process;
using PinDominatorCore.PDLibrary;
using PinDominatorCore.PDLibrary.Process;
using Unity;

namespace PinDominatorCore.PDFactories
{
    public interface IPdQueryScraperFactory : IQueryScraperFactory
    {
    }

    public class PdQueryScraperFactory : IPdQueryScraperFactory
    {
        private readonly IUnityContainer _unityContainer;

        public PdQueryScraperFactory(IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
        }

        public QueryScraper Create(JobProcess jobProcess)
        {
            var scraper = _unityContainer.Resolve<IPinterestScraperActionTables>();

            return new PinterestQueryScraper((IPdJobProcess) jobProcess, scraper.ScrapeWithQueriesActionTable,
                scraper.ScrapeWithoutQueriesActionTable);
        }
    }
}