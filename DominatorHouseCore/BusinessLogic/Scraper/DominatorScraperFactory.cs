#region

using CommonServiceLocator;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;

#endregion

namespace DominatorHouseCore.BusinessLogic.Scraper
{
    public class DominatorScraperFactory : IQueryScraperFactory
    {
        public QueryScraper Create(JobProcess jobProcess)
        {
            var scrapeProcess =
                InstanceProvider.GetInstance<IQueryScraperFactory>(jobProcess.SocialNetworks.ToString());
            return scrapeProcess.Create(jobProcess);
        }
    }
}