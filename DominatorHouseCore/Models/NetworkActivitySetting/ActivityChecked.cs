#region

using System.Text.RegularExpressions;
using DominatorHouseCore.Utility;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.Models.NetworkActivitySetting
{
    [ProtoContract]
    public class ActivityChecked : BindableBase
    {
        private string _activityType;

        [ProtoMember(1)]
        public string ActivityType
        {
            get => _activityType;
            set => SetProperty(ref _activityType, value);
        }

        private bool _isActivity;

        [ProtoMember(2)]
        public bool IsActivity
        {
            get => _isActivity;
            set => SetProperty(ref _isActivity, value);
        }


        public string DisplayActivity => Regex.Replace(ActivityType, "(\\B[A-Z])", " $1");
    }
}