#region

using System.ComponentModel;
using System.Runtime.CompilerServices;
using DominatorHouseCore.Annotations;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.Models.SocioPublisher
{
    [ProtoContract]
    public class SocinatorIntellisenseModel : INotifyPropertyChanged
    {
        private string _key = string.Empty;

        [ProtoMember(1)]
        public string Key
        {
            get => _key;
            set
            {
                if (_key == value)
                    return;
                _key = value;
                OnPropertyChanged(nameof(Key));
            }
        }

        private string _value;

        [ProtoMember(2)]
        public string Value
        {
            get => _value;
            set
            {
                if (_value == value)
                    return;
                _value = value;
                OnPropertyChanged(nameof(Value));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}