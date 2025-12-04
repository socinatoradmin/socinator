using DominatorHouseCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RedditDominatorUI.TabManager
{
    /// <summary>
    /// Interaction logic for EngageCommetTab.xaml
    /// </summary>
    public partial class EngageCommetTab : Window
    {
        public EngageCommetTab()
        {
            InitializeComponent();

            var tabItems = new List<TabItemTemplates>()
            {
                new TabItemTemplates
                {
                    Title=FindResource("Comment").ToString(),
                    //Content = new Lazy<UserControl>(new Comment())

                },
                 new TabItemTemplates
                {
                     Title=FindResource("langUnFollower?").ToString(),

                   //Content = new Lazy<UserControl>(UnFollowers.GetSingeltonObjectUnFollower)
                }
            };

            GrowFollowerTab.ItemsSource = tabItems;
        }
    }
}
