using System;
using System.Windows.Controls;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    ///     Interaction logic for CreateBoard.xaml
    /// </summary>
    public partial class CreateBoard : UserControl
    {
        public CreateBoard(ICreateBoardViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void CmbboxQueryTypeLists_OnDropDownClosed(object sender, EventArgs e)
        {
        }
    }
}