using System.Collections.ObjectModel;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace PinDominatorCore.PDModel
{
    [ProtoContract]
    public class ManageNoteModel : BindableBase
    {
        private string _filterText;

        private string _mediaPath = string.Empty;
        private int _serialNo;
        private string _tryText;

        public int SerialNo
        {
            get => _serialNo;
            set
            {
                if (value == _serialNo)
                    return;
                SetProperty(ref _serialNo, value);
            }
        }

        public string TryText
        {
            get => _tryText;
            set
            {
                if (value == _tryText)
                    return;
                SetProperty(ref _tryText, value);
            }
        }

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

        public string MediaPath
        {
            get => _mediaPath;
            set
            {
                if (value == _mediaPath)
                    return;
                SetProperty(ref _mediaPath, value);
            }
        }


        public ObservableCollection<QueryContent> SelectedQuery { get; set; } =
            new ObservableCollection<QueryContent>();


        public ObservableCollection<QueryContent> LstQueries { get; set; } = new ObservableCollection<QueryContent>();
    }
}