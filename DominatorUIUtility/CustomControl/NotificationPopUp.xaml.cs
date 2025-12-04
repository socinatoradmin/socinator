using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.FileManagers;
using System;
using System.Windows;
using System.Windows.Controls;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for NotificationPopUp.xaml
    /// </summary>
    public partial class NotificationPopUp : UserControl
    {
        public NotificationPopUp(InvoiceManager invoiceManager)
        {

            InitializeComponent();
            try
            {
                if (invoiceManager.InvoiceDetails.status.Equals("2"))
                {
                    if (!SocinatorKeyHelper.Key.IsUnSubscribed)
                        btn_unsubscribe.Visibility = Visibility.Visible;
                }
                if(invoiceManager.NetworkAccessDetails.license_expires.Date <= DateTime.Now.Date)
                {
                    if (!SocinatorKeyHelper.Key.IsSubscribed)
                        btn_subscribe.Visibility = Visibility.Visible;
                }
            }
            catch (Exception)
            {

            }
        }

        private void Btn_click_unsubscribe(object sender, RoutedEventArgs e)
        {
            var process = System.Diagnostics.Process.Start("https://www.paypal.com/myaccount/autopay/connect/");
            SocinatorKeyHelper.Key.IsUnSubscribed = true;
            SocinatorKeyHelper.SaveKey(SocinatorKeyHelper.Key);
            btn_unsubscribe.Visibility = Visibility.Hidden;
        }

        private void Btn_click_subscribe(object sender, RoutedEventArgs e)
        {
            var process = System.Diagnostics.Process.Start("https://socinator.com/amember/member");
            SocinatorKeyHelper.Key.IsSubscribed = true;
            SocinatorKeyHelper.SaveKey(SocinatorKeyHelper.Key);
            btn_subscribe.Visibility = Visibility.Hidden;
            btn_unsubscribe.Visibility = Visibility.Hidden;
        }

        private void Btn_click_support(object sender, RoutedEventArgs e)
        {
            var process = System.Diagnostics.Process.Start("skype:live:support_45094?chat");
        }
    }
}
