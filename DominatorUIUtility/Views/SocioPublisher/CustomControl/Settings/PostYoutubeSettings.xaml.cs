using DominatorHouseCore.Annotations;
using DominatorHouseCore.Models.SocioPublisher.Settings;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace DominatorUIUtility.Views.SocioPublisher.CustomControl.Settings
{
    /// <summary>
    /// Interaction logic for PostYoutubeSettings.xaml
    /// </summary>
    public partial class PostYoutubeSettings : UserControl, INotifyPropertyChanged
    {
        private PublisherPostSettings _publisherPostSettings;
        public PostYoutubeSettings()
        {
            InitializeComponent();
        }
        public PostYoutubeSettings(PublisherPostSettings publisherPostSettings) : this()
        {
            PublisherPostSettings = publisherPostSettings;
            MainGrid.DataContext = PublisherPostSettings.YTPostSettings;
        }

        public PublisherPostSettings PublisherPostSettings
        {
            get => _publisherPostSettings;
            set
            {
                _publisherPostSettings = value;
                OnPropertyChanged(nameof(PublisherPostSettings));
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
