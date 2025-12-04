#region

using System.Collections.Generic;
using DominatorHouseCore.Utility;

#endregion

namespace DominatorHouseCore.Models
{
    public class RevisionHistoryModel : BindableBase
    {
        private string _version = string.Empty;

        public string Version
        {
            get => _version;
            set
            {
                if (_version != null && value == _version)
                    return;
                SetProperty(ref _version, value);
            }
        }

        private string _revisionDate = string.Empty;

        public string RevisionDate
        {
            get => _revisionDate;
            set
            {
                if (_revisionDate != null && value == _revisionDate)
                    return;
                SetProperty(ref _revisionDate, value);
            }
        }

        private List<string> _lstContent = new List<string>();

        public List<string> LstContent
        {
            get => _lstContent;
            set
            {
                if (_lstContent != null && value == _lstContent)
                    return;
                SetProperty(ref _lstContent, value);
            }
        }
    }
}