using DominatorHouseCore.Models;

namespace QuoraDominatorCore.QdLibrary.Processors
{
    public interface IQueryProcessor
    {
        void Start(QueryInfo queryInfo);
    }
}