using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Models.SocioPublisher.Settings;

namespace DominatorUIUtility.Views.SocioPublisher.CustomControl.Settings
{
    /// <summary>
    ///     Interaction logic for PostTwitterSettings.xaml
    /// </summary>
    public partial class PostTwitterSettings : UserControl, INotifyPropertyChanged
    {
        private PublisherPostSettings _publisherPostSettings;

        public PostTwitterSettings()
        {
            InitializeComponent();
        }

        public PostTwitterSettings(PublisherPostSettings publisherPostSettings) : this()
        {
            PublisherPostSettings = publisherPostSettings;
            MainGrid.DataContext = PublisherPostSettings.TdPostSettings;
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