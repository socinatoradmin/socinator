using System.ComponentModel;
using System.Runtime.CompilerServices;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Enums;
using DominatorHouseCore.ViewModel;

namespace DominatorUIUtility.CustomControl
{
    /// <inheritdoc>
    ///     <cref></cref>
    /// </inheritdoc>
    /// <summary>
    ///     Interaction logic for LiveChat.xaml
    /// </summary>
    public partial class LiveChat : INotifyPropertyChanged
    {
        private LiveChatViewModel _liveChatViewModel;

        public LiveChat(SocialNetworks network)
        {
            InitializeComponent();

            LiveChatViewModel = new LiveChatViewModel(network);

            MainGrid.DataContext = LiveChatViewModel;
        }

        public LiveChatViewModel LiveChatViewModel
        {
            get => _liveChatViewModel;
            set
            {
                _liveChatViewModel = value;
                OnPropertyChanged(nameof(LiveChatViewModel));
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