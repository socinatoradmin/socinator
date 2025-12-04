using DominatorHouseCore.BusinessLogic.Scraper;
using DominatorHouseCore.Process;
using GramDominatorCore.GDLibrary;
using Unity;

namespace GramDominatorCore.Factories
{
    public interface IGdQueryScraperFactory: IQueryScraperFactory
    {
    }
    public class GDQueryScraperFactory : IGdQueryScraperFactory
    {
        private readonly IUnityContainer _unityContainer;

        public GDQueryScraperFactory(IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
        }
        public QueryScraper Create(JobProcess jobProcess)
        {
            var scraper = _unityContainer.Resolve<IInstagramScraperActionTables>();

            return new InstagramQueryScraper((IGdJobProcess)jobProcess, scraper.ScrapeWithQueriesActionTable,
                scraper.ScrapeWithoutQueriesActionTable);
        }
    }
}
