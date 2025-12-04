#region

using System.Collections.ObjectModel;
using DominatorHouseCore.Models;

#endregion

namespace DominatorHouseCore.Interfaces
{
    public interface ISearchQueryControl
    {
        ObservableCollection<QueryInfo> SavedQueries { get; set; }
    }
}