#region

using DominatorHouseCore.Utility;

#endregion

namespace DominatorHouseCore.Models.FacebookModels
{
    public class ManageCustomCommentsModel : BindableBase
    {
        /*private int _serialNo;

        public int SerialNo
        {
            get
            {
                return _serialNo;
            }
            set
            {
                if (value == _serialNo)
                    return;
                SetProperty(ref _serialNo, value);
            }
        }*/
        private string _commentText;

        public string CommentText
        {
            get => _commentText;
            set
            {
                if (value == _commentText)
                    return;
                SetProperty(ref _commentText, value);
            }
        }

        private string _filterText;

        public string FilterText
        {
            get => _filterText;
            set
            {
                if (value == _filterText)
                    return;
                SetProperty(ref _filterText, value);
            }
        }
    }
}