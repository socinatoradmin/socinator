using System.Windows.Controls;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    ///     Interaction logic for GroupMemberScraper.xaml
    /// </summary>
    public partial class GroupMemberScraper : UserControl
    {
        public GroupMemberScraper(IGroupMemberScraperViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}