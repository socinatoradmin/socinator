using System.Windows.Controls;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    ///     Interaction logic for WebpageLikerCommentor.xaml
    /// </summary>
    public partial class WebpageLikerCommentor : UserControl
    {
        public WebpageLikerCommentor(IWebpageLikerCommentorViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}