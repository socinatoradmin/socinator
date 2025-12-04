using DominatorHouseCore.Models;
using DominatorHouseCore.Models.FacebookModels;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDModel.CommonSettings;
using ProtoBuf;

namespace FaceDominatorCore.FDModel.LikerCommentorModel
{
    public interface IWebpageLikerCommentorModel
    {
        //        List<string> ListWebPageUrl { get; set; }
        //
        //
        //        ObservableCollectionBase<ManageCustomCommentsModel> SavedComments { get; set; }

        ManageCustomCommentsModel CurrentCommment { get; set; }

    }

    [ProtoContract]
    public class WebpageLikerCommentorModel : ModuleSetting, IWebpageLikerCommentorModel
    {

        [ProtoMember(1)]
        public override JobConfiguration JobConfiguration { get; set; }



        /*private List<string> _listWebPageUrl = new List<string>();
        [ProtoMember(2)]
        public List<string> ListWebPageUrl
        {
            get { return _listWebPageUrl; }
            set
            {
                if (value == _listWebPageUrl)
                    return;
                SetProperty(ref _listWebPageUrl, value);
            }
        }*/

        private ObservableCollectionBase<ManageCustomCommentsModel> _savedComments = new ObservableCollectionBase<ManageCustomCommentsModel>();
        [ProtoMember(3)]
        public ObservableCollectionBase<ManageCustomCommentsModel> SavedComments
        {
            get { return _savedComments; }
            set
            {
                SetProperty(ref _savedComments, value);
            }
        }

        private ManageCustomCommentsModel _currentCommment = new ManageCustomCommentsModel();
        [ProtoMember(4)]
        public ManageCustomCommentsModel CurrentCommment
        {
            get { return _currentCommment; }
            set
            {
                SetProperty(ref _currentCommment, value);
            }
        }

        // ReSharper disable once UnusedMember.Global
        public JobConfiguration SlowSpeed = new JobConfiguration()
        {
            ActivitiesPerDay = new RangeUtilities(10, 15),
            ActivitiesPerHour = new RangeUtilities(1, 2),
            ActivitiesPerWeek = new RangeUtilities(60, 90),
            ActivitiesPerJob = new RangeUtilities(1, 2),
            DelayBetweenJobs = new RangeUtilities(88, 133),
            DelayBetweenActivity = new RangeUtilities(30, 60),
            DelayBetweenAccounts = new RangeUtilities(0, 0)

        };

        // ReSharper disable once UnusedMember.Global
        public JobConfiguration MediumSpeed = new JobConfiguration()
        {
            ActivitiesPerDay = new RangeUtilities(27, 40),
            ActivitiesPerHour = new RangeUtilities(3, 4),
            ActivitiesPerWeek = new RangeUtilities(160, 240),
            ActivitiesPerJob = new RangeUtilities(3, 5),
            DelayBetweenJobs = new RangeUtilities(87, 131),
            DelayBetweenActivity = new RangeUtilities(23, 45),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        // ReSharper disable once UnusedMember.Global
        public JobConfiguration FastSpeed = new JobConfiguration()
        {
            ActivitiesPerDay = new RangeUtilities(53, 80),
            ActivitiesPerHour = new RangeUtilities(5, 8),
            ActivitiesPerWeek = new RangeUtilities(320, 480),
            ActivitiesPerJob = new RangeUtilities(7, 10),
            DelayBetweenJobs = new RangeUtilities(87, 130),
            DelayBetweenActivity = new RangeUtilities(15, 30),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };


        // ReSharper disable once IdentifierTypo
        // ReSharper disable once UnusedMember.Global
        public JobConfiguration SuperfastSpeed = new JobConfiguration()
        {
            ActivitiesPerDay = new RangeUtilities(67, 100),
            ActivitiesPerHour = new RangeUtilities(7, 10),
            ActivitiesPerWeek = new RangeUtilities(400, 600),
            ActivitiesPerJob = new RangeUtilities(8, 12),
            DelayBetweenJobs = new RangeUtilities(88, 132),
            DelayBetweenActivity = new RangeUtilities(8, 15),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };
    }
}