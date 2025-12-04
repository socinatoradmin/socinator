using DominatorHouseCore.Models;
using DominatorUIUtility.ViewModel.SocioPublisher;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace DominatorUIUtility.Views.SocioPublisher
{
    /// <summary>
    /// Interaction logic for MultipleImagePostDialog.xaml
    /// </summary>
    public partial class MultipleImagePostDialog : UserControl, INotifyPropertyChanged
    {
        private static MultipleImagePostDialog Instance;
        private MultipleImagePostViewModel imagePostViewModel;
        public MultipleImagePostDialog()
        {
            InitializeComponent();
            Instance = this;
        }

        public MultipleImagePostDialog(ObservableCollection<MessageMediaInfo> mediaList):this()
        {
            ImagePostViewModel = new MultipleImagePostViewModel(mediaList);
            MultipleImagePost.DataContext = ImagePostViewModel;
        }

        public static MultipleImagePostDialog GetMultipleImagePostInstance(ObservableCollection<MessageMediaInfo> MediaList)
        {
            return Instance ?? (Instance = new MultipleImagePostDialog(MediaList));
        }
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public MultipleImagePostViewModel ImagePostViewModel
        {
            get=>imagePostViewModel;
            set { imagePostViewModel = value;OnPropertyChanged(nameof(ImagePostViewModel)); }
            
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
