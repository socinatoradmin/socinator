#region

using DominatorHouseCore.Utility;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.Models.FacebookModels
{
    public interface IManageFriends
    {
//        bool IsAcceptRequest { get; set; }
//
//        bool IsCancelReceivedRequest { get; set; }

        int Count { get; set; }
    }

    public class ManageFriends : BindableBase, IManageFriends
    {
        private bool _isAcceptRequest;

        [ProtoMember(1)]
        public bool IsAcceptRequest
        {
            get => _isAcceptRequest;
            set
            {
                if (value == _isAcceptRequest)
                    return;
                SetProperty(ref _isAcceptRequest, value);
            }
        }

        private bool _isCancelReceivedRequest;

        [ProtoMember(2)]
        public bool IsCancelReceivedRequest
        {
            get => _isCancelReceivedRequest;
            set
            {
                if (value == _isCancelReceivedRequest)
                    return;
                SetProperty(ref _isCancelReceivedRequest, value);
            }
        }

        private int _count;

        [ProtoMember(3)]
        public int Count
        {
            get => _count;
            set
            {
                if (value == _count)
                    return;
                SetProperty(ref _count, value);
            }
        }
    }
}