#region

using System.Collections.ObjectModel;
using DominatorHouseCore.Settings;

#endregion

namespace DominatorHouseCore.Interfaces
{
    public interface ISearchControl
    {
        ObservableCollection<Queries> ListQuery { get; set; }
        ObservableCollection<string> QueryType { get; set; }
        string TypedQuery { get; set; }
        string SelectedQueryType { get; set; }
        bool CustomFiltersChecked { get; set; }
    }
}