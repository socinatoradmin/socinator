using System.Windows.Controls;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    ///     Interaction logic for GroupScraper.xaml
    /// </summary>
    public partial class GroupScraper : UserControl
    {
        public GroupScraper(IGroupScraperViewModel viewMdoel)
        {
            InitializeComponent();
            DataContext = viewMdoel;
        }
    }
}