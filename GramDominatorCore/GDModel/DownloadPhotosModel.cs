using System.Collections.Generic;
using DominatorHouseCore.Utility;
using System.Collections.ObjectModel;
using ProtoBuf;
using DominatorHouseCore.Models;

namespace GramDominatorCore.GDModel
{
    public interface IDownloadPhotosModel
    {
        #region IDownloadPhotosModel

       // bool isDownloadVideo { get; set; }

       // bool isDownloadImage { get; set; }

       // bool isDownloadAlbum { get; set; }

       // string DownloadedFolderPath { get; set; }

        #endregion
    }



    /// <summary>
    /// TODO: remove alias
    /// </summary>
    public class DownloadPhotosModel : ModuleSetting, IDownloadPhotosModel
    {

        #region Variables

        private bool _isDownloadVideo;

        private bool _isDownloadImage;

        private bool _isDownloadAlbum;

       // private bool _IsEngagementRate{ get;set; }

        private string _downloadedFolderPath = ConstantVariable.GetDownloadedMediaFolderPath;

        #endregion

        //public DownloadPhotosModel()
        //{
        //    //ListQueryType = Enum.GetNames(typeof(GdPostQuery)).ToList();
        //}

        public List<string> ListQueryType { get; set; } = new List<string>();

        public override UserFilterModel UserFilterModel { get; set; } = new UserFilterModel();

        public override PostFilterModel PostFilterModel { get; set; } = new PostFilterModel();


        #region Set Job Configuration speed
        public JobConfiguration SlowSpeed = new JobConfiguration()
        {
            ActivitiesPerDay = new RangeUtilities(666, 1000),
            ActivitiesPerHour = new RangeUtilities(66, 100),
            ActivitiesPerWeek = new RangeUtilities(4000, 6000),
            ActivitiesPerJob = new RangeUtilities(83, 125),
            DelayBetweenJobs = new RangeUtilities(20, 25),
            DelayBetweenActivity = new RangeUtilities(9, 12),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        public JobConfiguration MediumSpeed = new JobConfiguration()
        {
            ActivitiesPerDay = new RangeUtilities(3333, 5000),
            ActivitiesPerHour = new RangeUtilities(333, 500),
            ActivitiesPerWeek = new RangeUtilities(20000, 30000),
            ActivitiesPerJob = new RangeUtilities(416, 625),
            DelayBetweenJobs = new RangeUtilities(20, 25),
            DelayBetweenActivity = new RangeUtilities(6, 9),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        public JobConfiguration FastSpeed = new JobConfiguration()
        {
            ActivitiesPerDay = new RangeUtilities(6666, 10000),
            ActivitiesPerHour = new RangeUtilities(666, 1000),
            ActivitiesPerWeek = new RangeUtilities(40000, 60000),
            ActivitiesPerJob = new RangeUtilities(833, 1250),
            DelayBetweenJobs = new RangeUtilities(20, 25),
            DelayBetweenActivity = new RangeUtilities(3, 6),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };


        public JobConfiguration SuperfastSpeed = new JobConfiguration()
        {
            ActivitiesPerDay = new RangeUtilities(13333, 20000),
            ActivitiesPerHour = new RangeUtilities(1200, 3500),
            ActivitiesPerWeek = new RangeUtilities(80000, 120000),
            ActivitiesPerJob = new RangeUtilities(600, 1200),
            DelayBetweenJobs = new RangeUtilities(10, 20),
            DelayBetweenActivity = new RangeUtilities(0, 3),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        #endregion


        #region IDownloadPhotosModel

        public bool isDownloadVideo
        {
            get { return _isDownloadVideo; }
            set
            {
                if (value == _isDownloadVideo)
                    return;
                SetProperty(ref _isDownloadVideo, value);
            }
        }

        public string DownloadedFolderPath
        {
            get
            {
                return _downloadedFolderPath;
            }
            set
            {
                if (value == _downloadedFolderPath)
                    return;
                SetProperty(ref _downloadedFolderPath, value);
            }
        }

        public bool isDownloadImage
        {
            get { return _isDownloadImage; }
            set
            {
                if (value == _isDownloadImage)
                    return;
                SetProperty(ref _isDownloadImage, value);
            }
        }

        public bool isDownloadAlbum
        {
            get { return _isDownloadAlbum; }
            set
            {
                if (value == _isDownloadAlbum)
                    return;
                SetProperty(ref _isDownloadAlbum, value);
            }
        }

        private bool _IsSavePost;
        public bool IsSavePost
        {
            get { return _IsSavePost; }
            set
            {
                if (value == _IsSavePost)
                    return;
                SetProperty(ref _IsSavePost, value);
            }
        }
                         
        public class PostRequiredData : BindableBase
        {
            public string ItemName { get; set; }

            [ProtoMember(1)]
            private bool _isSelected;
            public bool IsSelected
            {
                get
                { return _isSelected; }
                set
                { SetProperty(ref _isSelected, value); }
            }
        }

     

        private ObservableCollection<PostRequiredData> _listUserRequiredData = new ObservableCollection<PostRequiredData>();
        public ObservableCollection<PostRequiredData> ListUserRequiredData
        {
            get { return _listUserRequiredData; }
            set { SetProperty(ref _listUserRequiredData, value); }
        }
        #endregion

    }
}
