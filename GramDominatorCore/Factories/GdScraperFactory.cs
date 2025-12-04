using DominatorHouseCore.BusinessLogic.Scraper;
using DominatorHouseCore.Process;
using GramDominatorCore.GDLibrary;


namespace GramDominatorCore.Factories
{
    //public class GdScraperFactory : IScraperFactory
    //{
    //    static GdScraperFactory _instance;

    //    public static GdScraperFactory Instance => _instance ?? (_instance = new GdScraperFactory());

    //    public AbstractQueryScraper Create(JobProcess jobProcess)
    //    {
    //        return new InstagramScraper((GdJobProcess)jobProcess);
    //    }


    //    public AbstractQueryScraper Create(JobProcess jobProcess)
    //    {
    //        return new InstagramScraper((GdJobProcess)jobProcess);
    //    }
    //}

    public class GDQueryScraperFactory : IQueryScraperFactory
    {
        static GDQueryScraperFactory _instance;

        public static GDQueryScraperFactory Instance => _instance ?? (_instance = new GDQueryScraperFactory());

        public QueryScraper Create(JobProcess jobProcess)
        {
            var instagramScraper = new InstagramScraperActionTables((GdJobProcess)jobProcess);

            return new InstagramQueryScraper((GdJobProcess)jobProcess, instagramScraper.ScrapeWithQueriesActionTable,
                instagramScraper.ScrapeWithoutQueriesActionTable);
        }
    }
}
