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

namespace QuoraDominatorUI.CustomControl
{
    /// <summary>
    /// Interaction logic for ManageBlacklistControl.xaml
    /// </summary>
    public partial class ManageBlacklistControl : UserControl
    {
        public ManageBlacklistControl()
        {
            InitializeComponent();
            MainGrid.DataContext = this;
        }
        public bool IsChkPrivateBlacklist
        {
            get { return (bool)GetValue(IsChkPrivateBlacklistProperty); }
            set { SetValue(IsChkPrivateBlacklistProperty, value); }
        }

        public static readonly DependencyProperty IsChkPrivateBlacklistProperty =
            DependencyProperty.Register("IsChkPrivateBlacklist", typeof(bool), typeof(ManageBlacklistControl), 
                new FrameworkPropertyMetadata(OnAvailableItemsChanged)
                {
                    BindsTwoWayByDefault = true
                });
        private static void OnAvailableItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        public bool IsChkGroupBlacklist
        {
            get { return (bool)GetValue(IsChkGroupBlacklistProperty); }
            set { SetValue(IsChkGroupBlacklistProperty, value); }
        }

        public static readonly DependencyProperty IsChkGroupBlacklistProperty =
            DependencyProperty.Register("IsChkGroupBlacklist", typeof(bool), typeof(ManageBlacklistControl),
                new FrameworkPropertyMetadata(OnAvailableItemsChanged)
                {
                    BindsTwoWayByDefault = true
                });

        public bool IsChkSkipPrivateBlacklist
        {
            get { return (bool)GetValue(IsChkSkipPrivateBlacklistProperty); }
            set { SetValue(IsChkSkipPrivateBlacklistProperty, value); }
        }

        public static readonly DependencyProperty IsChkSkipPrivateBlacklistProperty =
            DependencyProperty.Register("IsChkSkipPrivateBlacklist", typeof(bool), typeof(ManageBlacklistControl),
                new FrameworkPropertyMetadata(OnAvailableItemsChanged)
                {
                    BindsTwoWayByDefault = true
                });

        public bool IsChkSkipGroupBlacklist
        {
            get { return (bool)GetValue(IsChkSkipGroupBlacklistProperty); }
            set { SetValue(IsChkSkipGroupBlacklistProperty, value); }
        }

        public static readonly DependencyProperty IsChkSkipGroupBlacklistProperty =
            DependencyProperty.Register("IsChkSkipGroupBlacklist", typeof(bool), typeof(ManageBlacklistControl),
                new FrameworkPropertyMetadata(OnAvailableItemsChanged)
                {
                    BindsTwoWayByDefault = true
                });
    }
}
