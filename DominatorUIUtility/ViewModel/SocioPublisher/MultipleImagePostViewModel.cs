using DominatorHouseCore.Command;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace DominatorUIUtility.ViewModel.SocioPublisher
{
    public class MultipleImagePostViewModel: BindableBase
    {
        private ObservableCollection<MessageMediaInfo> mediaList;
        private MessageMediaInfo messageMedia=new MessageMediaInfo();
        public ICommand DeleteMedia { get; set; }
        public MultipleImagePostViewModel(ObservableCollection<MessageMediaInfo> mediaList)
        {
            MediaList = mediaList;
            DeleteMedia = new BaseCommand<object>(sender => true, DeleteMediaExecute);
        }

        private void DeleteMediaExecute(object sender)
        {
            if (SelectedMedia != null)
            {
                MediaList.RemoveAt(MediaList.IndexOf(SelectedMedia));
            }
        }

        public ObservableCollection<MessageMediaInfo> MediaList
        {
            get { return mediaList; }
            set { mediaList = value; OnPropertyChanged(nameof(MediaList)); }
        }
        public MessageMediaInfo SelectedMedia
        {
            get => messageMedia;
            set { messageMedia = value;OnPropertyChanged(nameof(SelectedMedia)); }
        }
    }
}
