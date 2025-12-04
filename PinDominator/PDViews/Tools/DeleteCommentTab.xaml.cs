using DominatorHouseCore.Models;
using PinDominator.PDViews.Tools.DeleteComment;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PinDominator.PDViews.Tools
{
    /// <summary>
    /// Interaction logic for DeleteCommentTab.xaml
    /// </summary>
    public partial class DeleteCommentTab : UserControl
    {
        public DeleteCommentTab()
        {
            InitializeComponent();
            var TabItems = new List<TabItemTemplates>()
            {
                new TabItemTemplates
                {
                    Title=FindResource("PDlangConfiguration").ToString(),
                    Content=new Lazy<UserControl>(DeleteCommentConfiguration.GetSingeltonObjectDeleteCommentConfiguration)
                },
                new TabItemTemplates
                {
                    Title=FindResource("PDlangReports").ToString(),
                    //Content=new Lazy<UserControl>(()=>new FollowReports())
                }
            };
            DeleteComment.ItemsSource = TabItems;
        }
    }
}
