#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Models.Publisher;
using DominatorHouseCore.Utility;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.Models.SocioPublisher
{
    [Serializable]
    [ProtoContract]
    public class PublisherCampaignStatusModel : INotifyPropertyChanged
    {
        public string CampaignId { get; set; } = string.Empty;

        private bool _isSelected;
        private int _destinationCount;
        private int _draftCount;
        private int _pendingCount;
        private int _publishedCount;
        private DateTime _createdDate;
        private DateTime? _startDate;
        private DateTime? _endDate;

        /// <summary>
        ///     Is campaign Selected
        /// </summary>
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }

        /// <summary>
        ///     To specify the campaign Name
        /// </summary>
        public string CampaignName { get; set; } = string.Empty;

        /// <summary>
        ///     To Specify the status for campaigns
        /// </summary>
        public PublisherCampaignStatus Status { get; set; } = PublisherCampaignStatus.Completed;

        /// <summary>
        ///     To Specify the destination count
        /// </summary>
        public int DestinationCount
        {
            get => _destinationCount;
            set
            {
                _destinationCount = value;
                OnPropertyChanged(nameof(DestinationCount));
            }
        }

        /// <summary>
        ///     To specify the draft count of postlists
        /// </summary>
        public int DraftCount
        {
            get => _draftCount;
            set
            {
                _draftCount = value;
                OnPropertyChanged(nameof(DraftCount));
            }
        }

        /// <summary>
        ///     To specify the pending count of postlists
        /// </summary>
        public int PendingCount
        {
            get => _pendingCount;
            set
            {
                _pendingCount = value;
                OnPropertyChanged(nameof(PendingCount));
            }
        }

        /// <summary>
        ///     To specify the published count of postlists
        /// </summary>
        public int PublishedCount
        {
            get => _publishedCount;
            set
            {
                _publishedCount = value;
                OnPropertyChanged(nameof(PublishedCount));
            }
        }

        /// <summary>
        ///     To specify the created date time
        /// </summary>
        public DateTime CreatedDate
        {
            get => _createdDate;
            set
            {
                _createdDate = value;
                OnPropertyChanged(nameof(CreatedDate));
            }
        }

        private DateTime _updatedTime;

        /// <summary>
        ///     To specifty the last modified date time
        /// </summary>
        public DateTime UpdatedTime
        {
            get => _updatedTime;
            set
            {
                _updatedTime = value;
                OnPropertyChanged(nameof(UpdatedTime));
            }
        }

        /// <summary>
        ///     To specify the start date of the campaign
        /// </summary>
        public DateTime? StartDate
        {
            get => _startDate;
            set
            {
                _startDate = value;
                OnPropertyChanged(nameof(StartDate));
            }
        }

        /// <summary>
        ///     To specify the end date of the campaign
        /// </summary>
        public DateTime? EndDate
        {
            get => _endDate;
            set
            {
                _endDate = value;
                OnPropertyChanged(nameof(EndDate));
            }
        }

        private bool _isDelayPostChecked;

        [ProtoMember(35)]
        public bool IsDelayPostChecked
        {
            get => _isDelayPostChecked;
            set
            {
                _isDelayPostChecked = value;
                OnPropertyChanged(nameof(IsDelayPostChecked));
            }
        }

        #region Job Scheduling

        /// <summary>
        ///     To specify the selected day for a campaign
        /// </summary>
        public List<ContentSelectGroup> ScheduledWeekday { get; set; } = new List<ContentSelectGroup>();

        /// <summary>
        ///     To specify whether campaign is start running with rotate day
        /// </summary>
        public bool IsRotateDayChecked { get; set; }

        /// <summary>
        ///     To specify the running time for a campaign
        /// </summary>
        public List<TimeSpan> SpecificRunningTime { get; set; } = new List<TimeSpan>();

        /// <summary>
        ///     To specify whether campaign is running with randomize the time everyday
        /// </summary>
        public bool IsRandomRunningTime { get; set; }

        /// <summary>
        ///     To specify how minutes publishing operation will take place
        /// </summary>
        public int MaximumTime { get; set; }

        /// <summary>
        ///     To specify the running time range of a campaign
        /// </summary>
        public TimeRange TimeRange { get; set; }

        /// <summary>
        ///     To specify whether take randon destinations
        /// </summary>
        public bool IsTakeRandomDestination { get; set; }

        /// <summary>
        ///     To specify While publihsing Send One Post For Each Destination
        /// </summary>
        public bool SendOnePostForEachDestination { get; set; }

        /// <summary>
        ///     To specify the total random destinations
        /// </summary>
        public int TotalRandomDestination { get; set; }

        /// <summary>
        ///     To specify the minimum destination per account
        /// </summary>
        public int MinRandomDestinationPerAccount { get; set; }

        #endregion

        /// <summary>
        ///     Generate campaign details with default values
        /// </summary>
        public void GenerateCampaign()
        {
            CampaignName = $"Campaign-{ConstantVariable.GetDateTime()}";
            CampaignId = Utilities.GetGuid();
            CreatedDate = DateTime.Today;
            StartDate = DateTime.Today;
            EndDate = DateTime.Today.AddDays(7);
        }

        /// <summary>
        ///     Generate campaign details with default values, but its uses default campaign name
        /// </summary>
        public void GenerateCloneCampaign(string name)
        {
            CampaignName = $"{name}-clone-{ConstantVariable.GetHourDateTime()}";
            CampaignId = Utilities.GetGuid();
            CreatedDate = DateTime.Now;
            IsSelected = false;
            UpdatedTime = DateTime.Now;
        }

        /// <summary>
        ///     Validate the start and end date time
        /// </summary>
        /// <returns></returns>
        public bool ValidDateTime()
        {
            if (StartDate == null || EndDate == null)
                return true;

            return StartDate <= EndDate;
        }

        [field: NonSerialized] public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}