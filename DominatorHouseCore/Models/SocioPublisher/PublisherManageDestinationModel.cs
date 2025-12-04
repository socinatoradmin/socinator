#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Utility;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.Models.SocioPublisher
{
    [ProtoContract]
    public class PublisherManageDestinationModel : INotifyPropertyChanged
    {
        private string _destinationId;
        private bool _isSelected;
        private int _accountCount;
        private int _pagesOrBoardsCount;
        private int _groupsCount;
        private int _wallsOrProfilesCount;
        private int _campaignsCount;
        private int _customDestinationCount;
        private string _destinationName;
        private List<string> _addUsedcampaignId = new List<string>();

        [ProtoMember(1)]
        public string DestinationId
        {
            get => _destinationId;
            set
            {
                if (_destinationId == value)
                    return;
                _destinationId = value;
                OnPropertyChanged(nameof(DestinationId));
            }
        }

        [ProtoMember(2)]
        public string DestinationName
        {
            get => _destinationName;
            set
            {
                if (_destinationName == value)
                    return;
                _destinationName = value;
                OnPropertyChanged(nameof(DestinationName));
            }
        }

        [ProtoMember(3)]
        public int AccountCount
        {
            get => _accountCount;
            set
            {
                if (_accountCount == value)
                    return;
                _accountCount = value;
                OnPropertyChanged(nameof(AccountCount));
            }
        }

        [ProtoMember(4)]
        public int PagesOrBoardsCount
        {
            get => _pagesOrBoardsCount;
            set
            {
                if (_pagesOrBoardsCount == value)
                    return;
                _pagesOrBoardsCount = value;
                OnPropertyChanged(nameof(PagesOrBoardsCount));
            }
        }

        [ProtoMember(5)]
        public int GroupsCount
        {
            get => _groupsCount;
            set
            {
                if (_groupsCount == value)
                    return;
                _groupsCount = value;
                OnPropertyChanged(nameof(GroupsCount));
            }
        }

        [ProtoMember(6)]
        public int WallsOrProfilesCount
        {
            get => _wallsOrProfilesCount;
            set
            {
                if (_wallsOrProfilesCount == value)
                    return;
                _wallsOrProfilesCount = value;
                OnPropertyChanged(nameof(WallsOrProfilesCount));
            }
        }

        [ProtoMember(7)]
        public int CampaignsCount
        {
            get => _campaignsCount;
            set
            {
                if (_campaignsCount == value)
                    return;
                _campaignsCount = value;
                OnPropertyChanged(nameof(CampaignsCount));
            }
        }

        [ProtoMember(8)] public DateTime CreatedDate { get; set; }

        [ProtoMember(9)]
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected == value)
                    return;
                _isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }


        [ProtoMember(10)]
        public int CustomDestinationsCount
        {
            get => _customDestinationCount;
            set
            {
                if (_customDestinationCount == value)
                    return;
                _customDestinationCount = value;
                OnPropertyChanged(nameof(CustomDestinationsCount));
            }
        }


        [ProtoMember(11)]
        public List<string> AddUsedCampaignId
        {
            get => _addUsedcampaignId;
            set
            {
                if (_addUsedcampaignId == value)
                    return;
                _addUsedcampaignId = value;
                OnPropertyChanged(nameof(AddUsedCampaignId));
            }
        }


        private bool _isAddNewGroups;

        [ProtoMember(12)]
        public bool IsAddNewGroups
        {
            get => _isAddNewGroups;
            set
            {
                if (_isAddNewGroups == value)
                    return;
                _isAddNewGroups = value;
                OnPropertyChanged(nameof(IsAddNewGroups));
            }
        }

        private bool _isRemoveGroupsRequiresValidation;

        [ProtoMember(13)]
        public bool IsRemoveGroupsRequiresValidation
        {
            get => _isRemoveGroupsRequiresValidation;
            set
            {
                if (_isRemoveGroupsRequiresValidation == value)
                    return;
                _isRemoveGroupsRequiresValidation = value;
                OnPropertyChanged(nameof(IsRemoveGroupsRequiresValidation));
            }
        }

        public void GenerateDestinations()
        {
            DestinationName = $"Destination-{ConstantVariable.GetDateTime()}";
            DestinationId = Utilities.GetGuid();
            CreatedDate = DateTime.Today;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public static void AddCampaignToDestinationList(IEnumerable<string> lstDestinationId, string campaignId)
        {
            #region Add campagins to destination list

            var allDestinations = ManageDestinationFileManager.GetAll();

            lstDestinationId.ForEach(destinationId =>
            {
                var currentDestination = allDestinations.FirstOrDefault(x => x.DestinationId == destinationId);

                if (currentDestination == null)
                    return;

                var index = allDestinations.IndexOf(currentDestination);
                currentDestination.AddUsedCampaignId.Add(campaignId);
                currentDestination.AddUsedCampaignId = currentDestination.AddUsedCampaignId.Distinct().ToList();
                currentDestination.CampaignsCount = currentDestination.AddUsedCampaignId.Count;
                allDestinations[index] = currentDestination;
            });

            ManageDestinationFileManager.UpdateDestinations(allDestinations);

            #endregion
        }

        public static void UpdateDestinationsGroupCount(string destinationId, int groupCount)
        {
            #region Update destination group Count

            var allDestinations = ManageDestinationFileManager.GetAll();

            var currentDestination = allDestinations.FirstOrDefault(x => x.DestinationId == destinationId);

            if (currentDestination == null)
                return;

            var index = allDestinations.IndexOf(currentDestination);

            currentDestination.GroupsCount = groupCount;

            allDestinations[index] = currentDestination;

            ManageDestinationFileManager.UpdateDestinations(allDestinations);

            #endregion
        }

        public static void RemoveDestinationFromCampaign(string campaignId)
        {
            #region Remove campagins from destination list

            var allDestinations = ManageDestinationFileManager.GetAll();

            foreach (var destination in allDestinations)
            {
                if (!destination.AddUsedCampaignId.Contains(campaignId))
                    continue;

                destination.AddUsedCampaignId.Remove(campaignId);
                destination.CampaignsCount = destination.AddUsedCampaignId.Count;
            }

            ManageDestinationFileManager.UpdateDestinations(allDestinations);

            #endregion
        }
    }
}