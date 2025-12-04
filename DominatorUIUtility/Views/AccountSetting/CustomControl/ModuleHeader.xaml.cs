using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DominatorUIUtility.Behaviours;
using Prism.Commands;

namespace DominatorUIUtility.Views.AccountSetting.CustomControl
{
    /// <summary>
    ///     Interaction logic for ModuleHeader.xaml
    /// </summary>
    public partial class ModuleHeader : UserControl
    {
        // Using a DependencyProperty as the backing store for Heading.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeadingProperty =
            DependencyProperty.Register("Heading", typeof(string), typeof(ModuleHeader),
                new PropertyMetadata(string.Empty));

        // Using a DependencyProperty as the backing store for IsExpanded.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register("IsExpanded", typeof(bool), typeof(ModuleHeader),
                new FrameworkPropertyMetadata(false)
                {
                    BindsTwoWayByDefault = true
                });

        // Using a DependencyProperty as the backing store for View.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewProperty =
            DependencyProperty.Register("View", typeof(object), typeof(ModuleHeader));

        public ModuleHeader()
        {
            InitializeComponent();
            Header.DataContext = this;
            ExpandCollapseAllCommand = new DelegateCommand(ExpandCollepseAll);
            HeaderHelper.UpdateToggleForNonQuery = UpdateToggleButton;
        }

        public string Heading
        {
            get => (string) GetValue(HeadingProperty);
            set => SetValue(HeadingProperty, value);
        }

        public bool IsExpanded
        {
            get => (bool) GetValue(IsExpandedProperty);
            set => SetValue(IsExpandedProperty, value);
        }

        public object View
        {
            get => GetValue(ViewProperty);
            set => SetValue(ViewProperty, value);
        }

        public ICommand ExpandCollapseAllCommand { get; set; }
        private bool IsClickedFromToggle { get; set; }


        public static void OnAvailableItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var newValue = e.NewValue;
        }

        private void ExpandCollepseAll()
        {
            IsClickedFromToggle = true;
            HeaderHelper.ExpandCollapseAllExpanderForActivity(View, IsExpanded);
        }

        private void UpdateToggleButton()
        {
            if (IsClickedFromToggle)
            {
                var isAllCollapsed = HeaderHelper.IsAllExpanderCollapseOrNot(View);
                IsExpanded = !isAllCollapsed;
            }
        }
    }
}