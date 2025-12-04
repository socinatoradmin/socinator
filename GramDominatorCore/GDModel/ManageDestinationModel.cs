//using DominatorHouseCore.Utility;
//using ProtoBuf;
//using System.Collections.ObjectModel;
//using System.ComponentModel;

//namespace GramDominatorCore.GDModel
//{
//    [ProtoContract]
//    public class ManageDestinationModel : BindableBase
//    {
//        private bool _isItemChecked;
//        [ProtoMember(1)]
//        public bool IsItemChecked
//        {
//            get
//            {
//                return _isItemChecked;
//            }
//            set
//            {
//                if (value == _isItemChecked)
//                    return;
//                SetProperty(ref _isItemChecked, value);
//            }
//        }
//        private string _name;
//        [ProtoMember(2)]
//        public string Name
//        {
//            get
//            {
//                return _name;
//            }
//            set
//            {
//                if (value == _name)
//                    return;
//                SetProperty(ref _name, value);
//            }
//        }
//        private string _profiles;
//        [ProtoMember(3)]
//        public string Profiles
//        {
//            get
//            {
//                return _profiles;
//            }
//            set
//            {
//                if (value == _profiles)
//                    return;
//                SetProperty(ref _profiles, value);
//            }
//        }
//        private string _pagesBoards;
//        [ProtoMember(4)]
//        public string PagesBoards
//        {
//            get
//            {
//                return _pagesBoards;
//            }
//            set
//            {
//                if (value == _pagesBoards)
//                    return;
//                SetProperty(ref _pagesBoards, value);
//            }
//        }
//        private string _groups;
//        [ProtoMember(5)]
//        public string Groups
//        {
//            get
//            {
//                return _groups;
//            }
//            set
//            {
//                if (value == _groups)
//                    return;
//                SetProperty(ref _groups, value);
//            }
//        }
//        private string _wallsProfiles;
//        [ProtoMember(6)]
//        public string WallsProfiles
//        {
//            get
//            {
//                return _wallsProfiles;
//            }
//            set
//            {
//                if (value == _wallsProfiles)
//                    return;
//                SetProperty(ref _wallsProfiles, value);
//            }
//        }
//        private int _order;
//        [ProtoMember(7)]
//        public int Order
//        {
//            get
//            {
//                return _order;
//            }
//            set
//            {
//                if (value == _order)
//                    return;
//                SetProperty(ref _order, value);
//            }
//        }
//        private int _noOfCampaigns;
//        [ProtoMember(8)]
//        public int NoOfCampaigns
//        {
//            get
//            {
//                return _noOfCampaigns;
//            }
//            set
//            {
//                if (value == _noOfCampaigns)
//                    return;
//                SetProperty(ref _noOfCampaigns, value);
//            }
//        }

//        private bool _isAutoSelectNewGroupsChecked;
//        [ProtoMember(9)]
//        public bool IsAutoSelectNewGroupsChecked
//        {
//            get
//            {
//                return _isAutoSelectNewGroupsChecked;
//            }
//            set
//            {
//                if (value == _isAutoSelectNewGroupsChecked)
//                    return;
//                SetProperty(ref _isAutoSelectNewGroupsChecked, value);
//            }
//        }
//        private bool _isUncheckGroupsChecked;
//        [ProtoMember(10)]
//        public bool IsUncheckGroupsChecked
//        {
//            get
//            {
//                return _isUncheckGroupsChecked;
//            }
//            set
//            {
//                if (value == _isUncheckGroupsChecked)
//                    return;
//                SetProperty(ref _isUncheckGroupsChecked, value);
//            }
//        }
//        private ObservableCollection<string> _lstDestinationTags = new ObservableCollection<string>();

//        public ObservableCollection<string> LstDestinationTags
//        {
//            get
//            {
//                return _lstDestinationTags;
//            }
//            set
//            {
//                if (_lstDestinationTags != null && value == _lstDestinationTags)
//                    return;
//                SetProperty(ref _lstDestinationTags, value);
//            }
//        }
//        private string _filter;
//        [ProtoMember(11)]
//        public string Filter
//        {
//            get
//            {
//                return _filter;
//            }
//            set
//            {
//                if (value == _filter)
//                    return;
//                SetProperty(ref _filter, value);
//            }
//        }
//        private bool _isFilterByTagChecked;
//        [ProtoMember(12)]
//        public bool IsFilterByTagChecked
//        {
//            get
//            {
//                return _isFilterByTagChecked;
//            }
//            set
//            {
//                if (value == _isFilterByTagChecked)
//                    return;
//                SetProperty(ref _isFilterByTagChecked, value);
//            }
//        }
//        private string _pages;
//        [ProtoMember(13)]
//        public string Pages
//        {
//            get
//            {
//                return _pages;
//            }
//            set
//            {
//                if (value == _pages)
//                    return;
//                SetProperty(ref _pages, value);
//            }
//        }

//        private string _walls;
//        [ProtoMember(14)]
//        public string Walls
//        {
//            get
//            {
//                return _walls;
//            }
//            set
//            {
//                if (value == _walls)
//                    return;
//                SetProperty(ref _walls, value);
//            }
//        }

//        private string _platform;
//        [ProtoMember(15)]
//        public string Platform
//        {
//            get
//            {
//                return _platform;
//            }
//            set
//            {
//                if (value == _platform)
//                    return;
//                SetProperty(ref _platform, value);
//            }
//        }
//        private ICollectionView _destinationCollection;
    
//        public ICollectionView DestinationCollection
//        {
//            get
//            {
//                return _destinationCollection;
//            }
//            set
//            {
//                if (value == _destinationCollection)
//                    return;
//                SetProperty(ref _destinationCollection, value);
//            }
//        }

//    }

//    [ProtoContract]
//    public class CreateDestination
//    {

//    }
//}
