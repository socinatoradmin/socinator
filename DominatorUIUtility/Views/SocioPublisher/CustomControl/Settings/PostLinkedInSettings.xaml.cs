using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Models.SocioPublisher.Settings;

namespace DominatorUIUtility.Views.SocioPublisher.CustomControl.Settings
{
    /// <summary>
    ///     Interaction logic for PostLinkedInSettings.xaml
    /// </summary>
    public partial class PostLinkedInSettings : UserControl, INotifyPropertyChanged
    {
        private PublisherPostSettings _publisherPostSettings;

        public PostLinkedInSettings()
        {
            InitializeComponent();
        }

        public PostLinkedInSettings(PublisherPostSettings publisherPostSettings) : this()
        {
            PublisherPostSettings = publisherPostSettings;
            MainGrid.DataContext = PublisherPostSettings.LdPostSettings;
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