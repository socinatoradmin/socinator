#region

using DominatorHouseCore.Utility;

#endregion

namespace DominatorHouseCore.Models
{
    public class ReportDetails : BindableBase
    {
        private string _ActionFrom;

        public string ActionFrom
        {
            get => _ActionFrom;
            set
            {
                if (value == _ActionFrom)
                    return;
                SetProperty(ref _ActionFrom, value);
            }
        }

        private string _Followed;

        public string Followed
        {
            get => _Followed;
            set
            {
                if (value == _Followed)
                    return;
                SetProperty(ref _Followed, value);
            }
        }

        public int _daterequested;

        public int DateRequested
        {
            get => _daterequested;
            set
            {
                if (value == _daterequested)
                    return;
                SetProperty(ref _daterequested, value);
            }
        }

        private string _status;

        public string Status
        {
            get => _status;
            set
            {
                if (value == _status)
                    return;
                SetProperty(ref _status, value);
            }
        }
    }
}