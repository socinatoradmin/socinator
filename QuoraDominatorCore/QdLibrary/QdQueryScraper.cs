using System;
using System.Collections.Generic;
using DominatorHouseCore.BusinessLogic.Scraper;
using DominatorHouseCore.Models;

namespace QuoraDominatorCore.QdLibrary
{
    public class QdQueryScraper : QueryScraper
    {
        public QdQueryScraper(IQdJobProcess jobProcess,
            Dictionary<string, Action<QueryInfo>> scrapeWithQueriesActionTable,
            Dictionary<string, Action> scrapeWithoutQueriesActionTable)
            : base(jobProcess, scrapeWithQueriesActionTable, scrapeWithoutQueriesActionTable)
        {
        }
    }
}