using System.Windows.Controls;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    ///     Interaction logic for Upvote.xaml
    /// </summary>
    public partial class Upvote : UserControl
    {
        public Upvote(IUpvoteViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}