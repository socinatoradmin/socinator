#region

using System;
using DominatorHouseCore.Utility;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.Models.SocioPublisher
{
    [Serializable]
    [ProtoContract]
    public class PublisherRssFeedModel : BindableBase
    {
        private string _feedUrl = string.Empty;

        [ProtoMember(1)]
        public string FeedUrl
        {
            get => _feedUrl;
            set
            {
                if (value == _feedUrl)
                    return;
                SetProperty(ref _feedUrl, value);
            }
        }

        private string _feedTemplate = "LangKeyRssFeedTemplate".FromResourceDictionary();

        [ProtoMember(2)]
        public string FeedTemplate
        {
            get => _feedTemplate;
            set
            {
                if (value == _feedTemplate)
                    return;
                SetProperty(ref _feedTemplate, value);
            }
        }

        private string _buttonContent = "LangKeySaveFeedUrl".FromResourceDictionary();

        [ProtoIgnore]
        public string ButtonContent
        {
            get => _buttonContent;
            set
            {
                if (value == _buttonContent)
                    return;
                SetProperty(ref _buttonContent, value);
            }
        }

        private PostDetailsModel _postDetailsModel = new PostDetailsModel();

        [ProtoMember(3)]
        public PostDetailsModel PostDetailsModel
        {
            get => _postDetailsModel;
            set
            {
                if (value == _postDetailsModel)
                    return;
                SetProperty(ref _postDetailsModel, value);
            }
        }
    }
}