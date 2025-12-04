using System.Windows.Controls;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    ///     Interaction logic for GroupJoiner.xaml
    /// </summary>
    public partial class GroupJoiner : UserControl
    {
        public GroupJoiner(IGroupJoinerViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}