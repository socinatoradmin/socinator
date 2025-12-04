using DominatorHouseCore.Command;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for SubscriptionPopUp.xaml
    /// </summary>
    public partial class SubscriptionPopUp : Window
    {
        public SubscribtionPopUpModel model;
        public ICommand LaunchAction {  get; set; }
        public SubscriptionPopUp()
        {
            InitializeComponent();
            LaunchAction = new BaseCommand<object>(sender => true, LaunchActionExecute);
        }

        private void LaunchActionExecute(object obj)
        {
            try
            {
                var value = obj as string;
                if(!string.IsNullOrEmpty(value))
                {
                    var url = value == "Stripe" ? model.StripeDashBoardUrl :
                        model.PayPalDashBoardUrl;
                    Process.Start(url);
                }
            }
            catch { }
        }

        public SubscriptionPopUp(SubscribtionPopUpModel popUpModel):this()
        {
            this.model = popUpModel;
            MainStackPanel.DataContext = model;
            PopupWindow.DataContext = this;
        }

        private void OpenDashBoard(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            try
            {
                Process.Start(e.Uri.AbsoluteUri);
                e.Handled=true;
            }
            catch { }
        }

        private void ClosePopUp(object sender, RoutedEventArgs e)
        {
            try
            {
                var days = (int)(model.expires - DateTime.Now).TotalDays;
                model.nextTimeToShow = DateTime.Now.AddDays(days);
                ConstantVariable.SavePopupInfo(model);
                this.Close();
            }
            catch { }
        }
    }
}
