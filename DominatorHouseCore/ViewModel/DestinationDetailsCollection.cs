#region

using System;
using DominatorHouseCore.Utility;

#endregion

namespace DominatorHouseCore.ViewModel
{
    public class DestinationDetailsCollection : BindableBase
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string MembersCount { get; set; }

        private bool _isSelected;

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected == value)
                    return;
                SetProperty(ref _isSelected, value);
            }
        }

        public string Status { get; set; }

        public DateTime AddedDateTime { get; set; }
    }
}