using System.ComponentModel;
using System.Runtime.CompilerServices;
using DominatorHouseCore.Annotations;
using DominatorUIUtility.ViewModel;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    ///     Interaction logic for BlacklistUserControl.xaml
    /// </summary>
    public partial class BlacklistUserControl : INotifyPropertyChanged
    {
        private BlackListViewModel _blackListViewModel = new BlackListViewModel();

        public BlacklistUserControl()
        {
            InitializeComponent();
            MainGrid.DataContext = BlackListViewModel;
            BlackListViewModel.InitializeData();
        }

        public BlackListViewModel BlackListViewModel
        {
            get => _blackListViewModel;
            set
            {
                if (_blackListViewModel == value)
                    return;
                _blackListViewModel = value;
                OnPropertyChanged(nameof(BlackListViewModel));
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