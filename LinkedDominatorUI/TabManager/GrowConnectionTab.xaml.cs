using System;
using System.Collections.Generic;
using System.Windows.Controls;
using DominatorHouseCore;
using DominatorHouseCore.Models;
using LinkedDominatorUI.LDViews.GrowConnection;

namespace LinkedDominatorUI.TabManager
{
    /// <summary>
    ///     Interaction logic for ConnectionTab.xaml
    /// </summary>
    public partial class GrowConnectionTab : UserControl
    {
        private static GrowConnectionTab objGrowConnectionTab;

        public GrowConnectionTab()
        {
            try
            {
                InitializeComponent();
                var tabItems = new List<TabItemTemplates>
                {
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyConnectionRequest").ToString(),
                        Content = new Lazy<UserControl>(ConnectionRequest.GetSingeltonObjectConnectionRequest)
                    },
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyAcceptConnectionRequest").ToString(),
                        Content = new Lazy<UserControl>(AcceptConnectionRequest
                            .GetSingeltonObjectAcceptConnectionRequest)
                    },
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyRemoveConnections").ToString(),
                        Content = new Lazy<UserControl>(RemoveConnection.GetSingeltonObjectRemoveConnection)
                    },
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyWithdrawConnectionRequest").ToString(),
                        Content = new Lazy<UserControl>(WithdrawConnectionRequest
                            .GetSingeltonObjectWithdrawConnectionRequest)
                    },
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyExportConnection").ToString(),
                        Content = new Lazy<UserControl>(ExportConnection.GetSingeltonObjectExportConnection)
                    },
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyFollowPages").ToString(),
                        Content = new Lazy<UserControl>(FollowPage.GetSingeltonObjectFollowPages)
                    },

                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeySendPageInvitations").ToString(),
                        Content = new Lazy<UserControl>(SendPageInvitation.GetSingletonSendPageInvitation)
                    },
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeySendGroupInvitations").ToString(),
                        Content = new Lazy<UserControl>(SendGroupInvitation.GetSingletonSendGroupInvitation)
                    },
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyBlockUser").ToString(),
                        Content = new Lazy<UserControl>(BlockUser.GetSingletonObjectBlockUser)
                    },
                    //new TabItemTemplates
                    //{
                    //    Title = FindResource("LangKeyEventInviter").ToString(),
                    //    Content = new Lazy<UserControl>(EventInviter.GetSingletonSendPageInvitation)
                    //}
                };
                GrowConnectionsTab.ItemsSource = tabItems;
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }


        public static GrowConnectionTab GetSingeltonObjectobGrowConnectionTab()
        {
            return objGrowConnectionTab ?? (objGrowConnectionTab = new GrowConnectionTab());
        }

        public void SetIndex(int index)
        {
            //GrowConnectionsTab is the name of this Tab
            GrowConnectionsTab.SelectedIndex = index;
        }
    }
}