#region

using System.Collections.Generic;
using System.Collections.ObjectModel;
using DominatorHouseCore.Models;

#endregion

namespace DominatorHouseCore.Interfaces.StartUp
{
    public interface IStartUpSearchQuery
    {
        List<string> ListQueryType { get; set; }
        ObservableCollection<QueryInfo> SavedQueries { get; set; }
    }
}