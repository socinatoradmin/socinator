using System;
using System.Windows;
using System.Windows.Input;
using DominatorHouseCore;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    ///     Interaction logic for AddUpdateAccountControl.xaml
    /// </summary>
    public partial class AddUpdateAccountControl:Window
    {
        /// <summary>
        ///     Constructor with dominatorAccountBaseModel as data context
        /// </summary>
        /// <param name="dominatorAccountBaseModelBinding">Pass the default values which is going to display in view page</param>
        /// <param name="title">Show the title of the user control, like Add account</param>
        /// <param name="actionButtonContent">Pass the action button content like Save</param>
        /// <param name="showAdvance">Pass true only if proxy ip contains values otherwise false</param>
        /// <param name="socialNetwork"></param>
        public AddUpdateAccountControl(DominatorAccountBaseModel dominatorAccountBaseModelBinding, string title,
            string actionButtonContent, bool showAdvance,
            SocialNetworks socialNetwork)
        {
            InitializeComponent();
            TitleTextBlock.Text =
                !string.IsNullOrEmpty(title) ? title : "LangKeyAddAccount".FromResourceDictionary();
            if (socialNetwork == SocialNetworks.Social)
                foreach (var item in SocinatorInitialize.GetRegisterNetwork())
                {
                    if (item == SocialNetworks.Social)
                        continue;
                    ComboBoxSocialNetworks.Items.Add(item);
                }
            else
                ComboBoxSocialNetworks.Items.Add(socialNetwork);

            btnSave.Content = !string.IsNullOrEmpty(actionButtonContent)
                ? actionButtonContent
                : "LangKeySave".FromResourceDictionary();
            CheckBoxShowAdvance.IsChecked = showAdvance;
            GridAdvanceOption.Visibility = showAdvance == false ? Visibility.Collapsed : Visibility.Visible;

            DominatorAccountBaseModel = dominatorAccountBaseModelBinding;
            UserControlAddUpdateAccount.DataContext = DominatorAccountBaseModel;
        }

        public DominatorAccountBaseModel DominatorAccountBaseModel { get; set; }
        public string JsonCookies { get; set; }
        public string JsonBrowserCookies { get; set; }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) btnSave.IsDefault = true;
        }

        private void SaveCopiedCookies_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(CopiedJsonCookies.Text?.Trim()))
                    return;

                JsonCookies = CopiedJsonCookies.Text;
                if (DominatorAccountBaseModel.AccountNetwork == SocialNetworks.Facebook)
                    JsonBrowserCookies = CopiedJsonCookies.Text;

                ToasterNotification.ShowSuccess("LangKeyCookiesSavedNowLogin".FromResourceDictionary());
            }
            catch (Exception ex)
            {
                if (ex.Message?.Contains(" parsing ") ?? false)
                {
                    ToasterNotification.ShowError("LangKeyCookiesNotInValidJsonText".FromResourceDictionary());
                }
                else
                {
                    ex.DebugLog();
                    ToasterNotification.ShowError("LangKeyOopsAnErrorOccured".FromResourceDictionary());
                }
            }
        }

        private void ClearCopiedCookies_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(JsonCookies))
            {
                ToasterNotification.ShowInfomation("LangKeyNoCookiesAdded".FromResourceDictionary());
                return;
            }

            JsonCookies = JsonBrowserCookies = CopiedJsonCookies.Text = null;
            ToasterNotification.ShowSuccess("LangKeyRemovedCookiesSuccessfully".FromResourceDictionary());
        }
        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void showAdvancebuttonClicked(object sender, RoutedEventArgs e)
        {
            UserControlAddUpdateAccount.Top = UserControlAddUpdateAccount.Top - 100;
            GridAdvanceOption.Visibility = Visibility.Visible;

        }

        private void showAdvancebuttonUnClicked(object sender, RoutedEventArgs e)
        {
            GridAdvanceOption.Visibility=Visibility.Collapsed;
            UserControlAddUpdateAccount.Top = UserControlAddUpdateAccount.Top + 100;
        }
    }
}