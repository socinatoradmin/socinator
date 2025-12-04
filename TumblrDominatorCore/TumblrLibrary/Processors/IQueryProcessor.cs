using DominatorHouseCore.Models;

namespace TumblrDominatorCore.TumblrLibrary.Processors
{
    internal interface IQueryProcessor
    {
        void Start(QueryInfo queryinfo);
    }
}