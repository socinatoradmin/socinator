#region

using System.Windows;
using DominatorHouseCore.Utility;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.Models.SocioPublisher
{
    [ProtoContract]
    public class PublisherMonitorFolderModel : BindableBase
    {
        private string _folderPath = string.Empty;

        [ProtoMember(1)]
        public string FolderPath
        {
            get => _folderPath;
            set
            {
                if (value == _folderPath)
                    return;
                SetProperty(ref _folderPath, value);
            }
        }

        private string _folderTemplate = Application.Current.FindResource("LangKeyFolderPathTemplate").ToString();

        [ProtoMember(2)]
        public string FolderTemplate
        {
            get => _folderTemplate;
            set
            {
                if (value == _folderTemplate)
                    return;
                SetProperty(ref _folderTemplate, value);
            }
        }

        private string _buttonContent = "LangKeySaveFolderPath".FromResourceDictionary();

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