#region

using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.Models
{
    [ProtoContract]
    public class NetworksActivityCustomizeModel : BindableBase
    {
        private ObservableCollection<EachNetworkActivityCustomizeModel> _networksActListCollection;

        [ProtoMember(1)]
        public ObservableCollection<EachNetworkActivityCustomizeModel> NetworksActListCollection
        {
            get => _networksActListCollection;
            set => SetProperty(ref _networksActListCollection, value);
        }
    }

    [ProtoContract]
    public class EachNetworkActivityCustomizeModel : BindableBase
    {
        private SocialNetworks _socialNetwork;

        [ProtoMember(1)]
        public SocialNetworks SocialNetwork
        {
            get => _socialNetwork;
            set => SetProperty(ref _socialNetwork, value);
        }

        private ObservableCollection<NetworkCustomizeActivityTypeModel> _networkActivityTypeModelCollections =
            new ObservableCollection<NetworkCustomizeActivityTypeModel>();

        [ProtoMember(2)]
        public ObservableCollection<NetworkCustomizeActivityTypeModel> NetworkActivityTypeModelCollections
        {
            get => _networkActivityTypeModelCollections;
            set => SetProperty(ref _networkActivityTypeModelCollections, value);
        }
    }

    [ProtoContract]
    public class NetworkCustomizeActivityTypeModel : BindableBase
    {
        private SocialNetworks _network;

        [ProtoMember(1)]
        public SocialNetworks Network
        {
            get => _network;
            set => SetProperty(ref _network, value);
        }

        private ActivityType _title;

        [ProtoMember(2)]
        public ActivityType Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private string _activityTitle;

        [ProtoMember(3)]
        public string ActivityTitle
        {
            get
            {
                _activityTitle = Regex.Replace(Title.ToString(), "(\\B[A-Z])", " $1");
                return _activityTitle;
            }
            set => SetProperty(ref _activityTitle, value);
        }

        private bool _isSelected;

        [ProtoMember(4)]
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }
    }
}