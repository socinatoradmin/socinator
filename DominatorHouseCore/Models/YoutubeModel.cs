#region

using DominatorHouseCore.Utility;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.Models
{
    [ProtoContract]
    public class YoutubeModel : BindableBase
    {
        private int _limitNumberOfSimultaneousWatchVideos = 5;

        [ProtoMember(1)]
        public int LimitNumberOfSimultaneousWatchVideos
        {
            get => _limitNumberOfSimultaneousWatchVideos;
            set => SetProperty(ref _limitNumberOfSimultaneousWatchVideos, value);
        }

        private bool _isCampainWiseUnique = true;

        [ProtoMember(2)]
        public bool IsCampainWiseUnique
        {
            get => _isCampainWiseUnique;
            set
            {
                if (value)
                    IsAccountWiseUnique = false;
                SetProperty(ref _isCampainWiseUnique, value);
            }
        }

        private bool _isAccountWiseUnique = true;

        [ProtoMember(3)]
        public bool IsAccountWiseUnique
        {
            get => _isAccountWiseUnique;
            set
            {
                if (value)
                    IsCampainWiseUnique = false;
                SetProperty(ref _isAccountWiseUnique, value);
            }
        }

        private int _StopRunQueryIfNActivityFailed = 20;

        [ProtoMember(4)]
        public int StopRunQueryIfNActivityFailed
        {
            get => _StopRunQueryIfNActivityFailed;
            set => SetProperty(ref _StopRunQueryIfNActivityFailed, value);
        }

        private int _timeoutToSolveCaptchaManually = 25;

        [ProtoMember(5)]
        public int TimeoutToSolveCaptchaManually
        {
            get => _timeoutToSolveCaptchaManually;
            set => SetProperty(ref _timeoutToSolveCaptchaManually, value);
        }

        private bool _isCheckActivitiesOnNPost;

        [ProtoMember(6)]
        public bool IsCheckActivitiesOnNPost
        {
            get => _isCheckActivitiesOnNPost;
            set => SetProperty(ref _isCheckActivitiesOnNPost, value);
        }

        private RangeUtilities _activitiesOnNPost = new RangeUtilities(5, 10);

        [ProtoMember(7)]
        public RangeUtilities ActivitiesOnNPost
        {
            get => _activitiesOnNPost;
            set => SetProperty(ref _activitiesOnNPost, value);
        }
    }
}