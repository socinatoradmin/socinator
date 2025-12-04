using DominatorHouseCore.Models;

namespace PinDominatorCore.PDLibrary.Processors
{
    public interface IQueryProcessor
    {
        void Start(QueryInfo queryInfo);
    }
}