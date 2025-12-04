using DominatorHouseCore.BusinessLogic.Scraper;
using DominatorHouseCore.Process;
using LinkedDominatorCore.LDLibrary;
using Unity;

namespace LinkedDominatorCore.Factories
{
    public interface ILdQueryScraperFactory : IQueryScraperFactory
    {
    }


    public class LdQueryScraperFactory : ILdQueryScraperFactory
    {
        private readonly IUnityContainer _unityContainer;

        public LdQueryScraperFactory(IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
        }

        public QueryScraper Create(JobProcess jobProcess)
        {
            var linkedInScraper = _unityContainer.Resolve<ILinkedInScraperActionTables>();

            return new LinkedinQueryScraper(jobProcess, linkedInScraper.ScrapeWithQueriesActionTable,
                linkedInScraper.ScrapeWithoutQueriesActionTable);
        }
    }
}