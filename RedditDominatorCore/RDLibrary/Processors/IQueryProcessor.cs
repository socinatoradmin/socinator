using DominatorHouseCore.Models;

namespace RedditDominatorCore.RDLibrary.Processors
{
    public interface IQueryProcessor
    {
        void Start(QueryInfo queryInfo);
    }
}