using System;
using System.Collections.Generic;
using System.Windows.Controls;
using DominatorHouseCore;
using DominatorHouseCore.Models;
using LinkedDominatorUI.LDViews.Profilling;

namespace LinkedDominatorUI.TabManager
{
    /// <summary>
    ///     Interaction logic for ProfillingTab.xaml
    /// </summary>
    public partial class ProfillingTab : UserControl
    {
        private static ProfillingTab objProfillingTab;

        public ProfillingTab()
        {
            try
            {
                InitializeComponent();

                var tab_items = new List<TabItemTemplates>
                {
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyProfileEndorsement").ToString(),
                        Content = new Lazy<UserControl>(() => ProfileEndorsement.GetSingeltonObjectProfileEndorsement())
                    }
                };
                ProfillingTabs.ItemsSource = tab_items;
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }


        public static ProfillingTab GetSingeltonObjectProfillingTab()
        {
            return objProfillingTab ?? (objProfillingTab = new ProfillingTab());
        }

        public void SetIndex(int index)
        {
            //GrowConnectionsTab is the name of this Tab
            ProfillingTabs.SelectedIndex = index;
        }
    }
}