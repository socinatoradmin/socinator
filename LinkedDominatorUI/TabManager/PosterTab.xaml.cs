using System;
using System.Collections.Generic;
using System.Windows.Controls;
using DominatorHouseCore;
using DominatorHouseCore.Models;
using LinkedDominatorUI.LDViews.Poster;

namespace LinkedDominatorUI.TabManager
{
    /// <summary>
    ///     Interaction logic for PosterTab.xaml
    /// </summary>
    public partial class PosterTab : UserControl
    {
        private static PosterTab objPosterTab;

        public PosterTab()
        {
            try
            {
                InitializeComponent();
                var tab_items = new List<TabItemTemplates>
                {
                    new TabItemTemplates
                    {
                        Title = FindResource("LangKeyLinkedinPoster").ToString(),
                        Content = new Lazy<UserControl>(() => new LinkedinPoster())
                    }
                };
                PosterTabs.ItemsSource = tab_items;
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }


        public static PosterTab GetSingeltonObjectPosterTab()
        {
            return objPosterTab ?? (objPosterTab = new PosterTab());
        }

        public void SetIndex(int index)
        {
            //GrowConnectionsTab is the name of this Tab
            PosterTabs.SelectedIndex = index;
        }
    }
}