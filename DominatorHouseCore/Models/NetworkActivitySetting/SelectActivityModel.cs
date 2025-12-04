#region

using System.Collections.ObjectModel;
using DominatorHouseCore.Utility;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.Models.NetworkActivitySetting
{
    [ProtoContract]
    public class SelectActivityModel : BindableBase
    {
        private ObservableCollection<ActivityChecked> _lstNetworkActivityType =
            new ObservableCollection<ActivityChecked>();

        [ProtoMember(1)]
        public ObservableCollection<ActivityChecked> LstNetworkActivityType
        {
            get => _lstNetworkActivityType;
            set => SetProperty(ref _lstNetworkActivityType, value);
        }
    }
}