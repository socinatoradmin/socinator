using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Models.SocioPublisher.Settings;

namespace DominatorUIUtility.Views.SocioPublisher.CustomControl.Settings
{
    /// <summary>
    ///     Interaction logic for PostTumblrSettings.xaml
    /// </summary>
    public partial class PostTumblrSettings : UserControl, INotifyPropertyChanged
    {
        private PublisherPostSettings _publisherPostSettings;

        public PostTumblrSettings()
        {
            InitializeComponent();
        }


        public PostTumblrSettings(PublisherPostSettings publisherPostSettings) : this()
        {
            PublisherPostSettings = publisherPostSettings;
            MainGrid.DataContext = PublisherPostSettings.TumblrPostSettings;
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