using System.Windows.Controls;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    ///     Interaction logic for EventInviter.xaml
    /// </summary>
    public partial class EventInviter : UserControl
    {
        public EventInviter(IEventInviterViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}