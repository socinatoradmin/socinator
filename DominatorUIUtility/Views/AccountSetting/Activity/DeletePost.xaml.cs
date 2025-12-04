using System.Windows.Controls;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    ///     Interaction logic for DeletePost.xaml
    /// </summary>
    public partial class DeletePost : UserControl
    {
        public DeletePost(IDeletePostViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}