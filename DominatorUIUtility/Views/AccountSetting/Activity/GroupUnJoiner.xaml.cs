using System.Windows.Controls;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    ///     Interaction logic for GroupUnJoiner.xaml
    /// </summary>
    public partial class GroupUnJoiner : UserControl
    {
        public GroupUnJoiner(IGroupUnJoinerViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}