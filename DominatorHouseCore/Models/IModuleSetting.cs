#region

using System.Collections.ObjectModel;

#endregion

namespace DominatorHouseCore.Models
{
    public interface IModuleSetting
    {
        JobConfiguration JobConfiguration { get; }
        ObservableCollection<QueryInfo> SavedQueries { get; }
    }
}