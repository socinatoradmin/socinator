#region

using System;
using DominatorHouseCore.Interfaces.SocioPublisher;
using DominatorHouseCore.Utility;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.Models.SocioPublisher.Settings
{
    [Serializable]
    [ProtoContract]
    public class TumblrPostSettings : BindableBase, ITumblrSettings
    {
        private bool _isTagUser;
        private string _tagUserList = string.Empty;

        [ProtoMember(1)]
        public bool IsTagUser
        {
            get => _isTagUser;
            set
            {
                if (_isTagUser == value)
                    return;
                SetProperty(ref _isTagUser, value);
            }
        }

        [ProtoMember(2)]
        public string TagUserList
        {
            get => _tagUserList;
            set
            {
                if (_tagUserList == value)
                    return;
                SetProperty(ref _tagUserList, value);
            }
        }
    }
}