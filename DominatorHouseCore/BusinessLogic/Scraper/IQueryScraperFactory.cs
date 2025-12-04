#region

using DominatorHouseCore.Process;

#endregion

namespace DominatorHouseCore.BusinessLogic.Scraper
{
    //public interface IQueryScraperFactory
    //{
    //    AbstractQueryScraper Create(JobProcess jobProcess);
    //}

    public interface IQueryScraperFactory
    {
        QueryScraper Create(JobProcess jobProcess);
    }
}