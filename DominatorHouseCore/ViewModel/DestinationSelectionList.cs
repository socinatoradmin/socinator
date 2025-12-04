#region

using System.Collections.Generic;

#endregion

namespace DominatorHouseCore.ViewModel
{
    public class DestinationSelectionList
    {
        public int SelectedItemCount { get; set; }

        public int TotalItemCount { get; set; }

        public IEnumerable<DestinationDetailsCollection> DestinationDetailsCollection { get; set; }
    }
}