#region

using DominatorHouseCore.Utility;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.Settings
{
    [ProtoContract]
    public class BlacklistSettings : BindableBase
    {
        private bool _addInteractedBlacklist;

        [ProtoMember(1)]
        public bool AddInteractedBlacklist
        {
            get => _addInteractedBlacklist;
            set
            {
                if (value == _addInteractedBlacklist) return;
                SetProperty(ref _addInteractedBlacklist, value);
            }
        }

        private bool _pilterBlacklist;

        [ProtoMember(2)]
        public bool FilterBlacklist
        {
            get => _pilterBlacklist;
            set
            {
                if (value == _pilterBlacklist)
                    return;
                SetProperty(ref _pilterBlacklist, value);
            }
        }
    }
}