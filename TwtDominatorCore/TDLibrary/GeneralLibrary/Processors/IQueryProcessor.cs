using DominatorHouseCore.Models;

namespace TwtDominatorCore.TDLibrary.GeneralLibrary.Processors
{
    internal interface IQueryProcessor
    {
        void Start(QueryInfo queryInfo);
    }
}