using System.Windows.Controls;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    ///     Interaction logic for AutoReplyToNewMessage.xaml
    /// </summary>
    public partial class AutoReplyToNewMessage : UserControl
    {
        private IAutoReplyToNewMessageViewModel ObjViewModel;

        public AutoReplyToNewMessage(IAutoReplyToNewMessageViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            ObjViewModel = viewModel;
        }
    }
}