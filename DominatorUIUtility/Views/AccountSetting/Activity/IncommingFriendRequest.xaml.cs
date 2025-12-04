using System.Windows;
using System.Windows.Controls;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    ///     Interaction logic for IncommingFriendRequest.xaml
    /// </summary>
    public partial class IncommingFriendRequest : UserControl
    {
        private readonly IIncommingFriendRequestViewModel _viewModel;

        public IncommingFriendRequest(IIncommingFriendRequestViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = viewModel;
        }

        private void Postoptions_Checked(object sender, RoutedEventArgs e)
        {
            if (_viewModel.Count < 2)
                _viewModel.Count++;
        }

        private void Postoptions_Unchecked(object sender, RoutedEventArgs e)
        {
            if (_viewModel.Count > 0)
                _viewModel.Count--;
        }
    }
}