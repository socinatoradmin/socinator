#region

using DominatorHouseCore.Utility;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.Models.SocioPublisher
{
    [ProtoContract]
    public class PublisherCustomDestinationModel : BindableBase
    {
        private string _destinationType = string.Empty;

        [ProtoMember(1)]
        public string DestinationType
        {
            get => _destinationType;
            set
            {
                if (_destinationType == value)
                    return;
                SetProperty(ref _destinationType, value);
            }
        }

        private string _destinationValue = string.Empty;

        [ProtoMember(2)]
        public string DestinationValue
        {
            get => _destinationValue;
            set
            {
                if (_destinationValue == value)
                    return;
                SetProperty(ref _destinationValue, value);
            }
        }
    }
}