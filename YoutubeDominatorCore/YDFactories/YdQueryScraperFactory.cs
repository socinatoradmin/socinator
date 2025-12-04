using DominatorHouseCore.BusinessLogic.Scraper;
using DominatorHouseCore.Process;
using Unity;
using YoutubeDominatorCore.YoutubeLibrary.Processes;
using YoutubeDominatorCore.YoutubeLibrary.YdFunctions;

namespace YoutubeDominatorCore.YDFactories
{
    public interface IYdQueryScraperFactory : IQueryScraperFactory
    {
    }

    public class YdQueryScraperFactory : IYdQueryScraperFactory
    {
        private readonly IUnityContainer _unityContainer;

        public YdQueryScraperFactory(IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
        }

        public QueryScraper Create(JobProcess jobProcess)
        {
            var youtubeScraper = _unityContainer.Resolve<IYoutubeScraperActionTables>();

            return new YoutubeQueryScraper((IYdJobProcess)jobProcess, youtubeScraper.ScrapeWithQueriesActionTable,
                youtubeScraper.ScrapeWithoutQueriesActionTable);
        }
    }
}