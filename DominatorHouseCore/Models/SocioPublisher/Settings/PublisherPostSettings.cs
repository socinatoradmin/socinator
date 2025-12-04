#region

using System;
using DominatorHouseCore.Utility;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.Models.SocioPublisher.Settings
{
    [Serializable]
    [ProtoContract]
    public class PublisherPostSettings : BindableBase
    {
        private GeneralPostSettings _generalPostSettings = new GeneralPostSettings();
        private FdPostSettings _fdPostSettings = new FdPostSettings();
        private GdPostSettings _gdPostSettings = new GdPostSettings();
        private TdPostSettings _tdPostSettings = new TdPostSettings();
        private LdPostSettings _ldPostSettings = new LdPostSettings();
        private TumblrPostSettings _tumblrPostSettings = new TumblrPostSettings();
        private RedditPostSetting _redditPostSetting = new RedditPostSetting();
        private YTPostSettings _YTPostSettings = new YTPostSettings();
        [ProtoMember(1)]
        public GeneralPostSettings GeneralPostSettings
        {
            get => _generalPostSettings;
            set
            {
                if (_generalPostSettings == value)
                    return;

                SetProperty(ref _generalPostSettings, value);
            }
        }

        [ProtoMember(2)]
        public FdPostSettings FdPostSettings
        {
            get => _fdPostSettings;
            set => SetProperty(ref _fdPostSettings, value);
        }

        [ProtoMember(3)]
        public GdPostSettings GdPostSettings
        {
            get => _gdPostSettings;
            set => SetProperty(ref _gdPostSettings, value);
        }

        [ProtoMember(4)]
        public TdPostSettings TdPostSettings
        {
            get => _tdPostSettings;
            set => SetProperty(ref _tdPostSettings, value);
        }

        [ProtoMember(5)]
        public LdPostSettings LdPostSettings
        {
            get => _ldPostSettings;
            set => SetProperty(ref _ldPostSettings, value);
        }

        [ProtoMember(6)]
        public TumblrPostSettings TumblrPostSettings
        {
            get => _tumblrPostSettings;
            set => SetProperty(ref _tumblrPostSettings, value);
        }

        [ProtoMember(7)]
        public RedditPostSetting RedditPostSetting
        {
            get => _redditPostSetting;
            set => SetProperty(ref _redditPostSetting, value);
        }
        [ProtoMember(8)]
        public YTPostSettings YTPostSettings
        {
            get => _YTPostSettings;
            set => SetProperty(ref _YTPostSettings, value);
        }
    }
}