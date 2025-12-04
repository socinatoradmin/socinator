using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using DominatorHouseCore.Annotations;

namespace DominatorUIUtility.Views.Publisher
{
    /// <summary>
    ///     Interaction logic for ManageDestinationIndex.xaml
    /// </summary>
    public partial class ManageDestinationIndex : UserControl, INotifyPropertyChanged
    {
        private static ManageDestinationIndex _instance;
        private UserControl _selectedControl = new ManageDestination();

        private ManageDestinationIndex()
        {
            InitializeComponent();
            DestinationIndex.DataContext = this;
        }

        public UserControl SelectedControl
        {
            get => _selectedControl;
            set
            {
                _selectedControl = value;
                OnPropertyChanged(nameof(SelectedControl));
            }
        }

        public static ManageDestinationIndex Instance { get; set; }
            = _instance ?? (_instance = new ManageDestinationIndex());

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}