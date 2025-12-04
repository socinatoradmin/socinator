using DominatorHouseCore.Utility;
using ProtoBuf;
using System;

namespace DominatorHouseCore.Models.SocioPublisher.Settings
{
    [Serializable]
    [ProtoContract]
    public class YTPostSettings: BindableBase, IYTPostSettings
    {
        private bool _IsCommunityPost;
        [ProtoMember(1)]
        public bool IsCommunityPost
        {
            get => _IsCommunityPost;
            set => SetProperty(ref _IsCommunityPost, value,nameof(IsCommunityPost));
        }
    }
    public interface IYTPostSettings
    {
        bool IsCommunityPost {  get; set; }
    }
}
