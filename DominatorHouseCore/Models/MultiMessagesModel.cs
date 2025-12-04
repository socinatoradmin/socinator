#region

using System.Collections.ObjectModel;
using DominatorHouseCore.Utility;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.Models
{
    [ProtoContract]
    public class MultiMessagesModel : BindableBase
    {
        private ObservableCollection<string> _lstMessages = new ObservableCollection<string>();

        [ProtoMember(1)]
        public ObservableCollection<string> LstMessages
        {
            get => _lstMessages;
            set
            {
                if (_lstMessages != null)
                    return;
                SetProperty(ref _lstMessages, value);
            }
        }

        private int _noOfmessage = 2;

        public int NoOfMessages
        {
            get => _noOfmessage;
            set
            {
                if (_noOfmessage == value)
                    return;
                SetProperty(ref _noOfmessage, value);
            }
        }
    }
}