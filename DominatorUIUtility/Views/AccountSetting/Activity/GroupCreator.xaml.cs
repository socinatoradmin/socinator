using System.Windows.Controls;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    ///     Interaction logic for GroupCreator.xaml
    /// </summary>
    public partial class GroupCreator : UserControl
    {
        public GroupCreator(IGroupCreatorViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}