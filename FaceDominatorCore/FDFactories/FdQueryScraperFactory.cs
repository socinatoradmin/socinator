using DominatorHouseCore.BusinessLogic.Scraper;
using DominatorHouseCore.Process;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDLibrary.FdProcesses;
using Unity;

namespace FaceDominatorCore.FDFactories
{
    public interface IFdQueryScraperFactory : IQueryScraperFactory
    {

    }

    public class FdQueryScraperFactory : IFdQueryScraperFactory
    {
        private readonly IUnityContainer _unityContainer;

        public FdQueryScraperFactory(IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
        }

        public QueryScraper Create(JobProcess jobProcess)
        {
            var facebookScraper = new FacebookScraperActionTables(jobProcess, _unityContainer);

            return new FacebookQueryScraper((IFdJobProcess)jobProcess, facebookScraper.ScrapeWithQueriesActionTable,
                facebookScraper.ScrapeWithoutQueriesActionTable);
        }
    }

}
