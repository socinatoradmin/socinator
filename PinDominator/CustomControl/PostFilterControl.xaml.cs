using DominatorHouseCore.Command;
using DominatorHouseCore.LogHelper;
using PinDominatorCore.PDModel;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace PinDominator.CustomControl
{
    /// <summary>
    ///     Interaction logic for PostFilterControl.xaml
    /// </summary>
    public partial class PostFilterControl
    {
        // Using a DependencyProperty as the backing store for PostFilter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PostFilterProperty =
            DependencyProperty.Register("PostFilter", typeof(PostFilterModel), typeof(PostFilterControl),
                new FrameworkPropertyMetadata()
                {
                    BindsTwoWayByDefault = true
                });

        // Using a DependencyProperty as the backing store for PostFilter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommentVisibilityProperty =
            DependencyProperty.Register("CommentVisibility", typeof(Visibility), typeof(PostFilterControl),
                new FrameworkPropertyMetadata()
                {
                    BindsTwoWayByDefault = true
                });

        public PostFilterControl()
        {
            InitializeComponent();
            MainGrid.DataContext = this;
            SaveBlacklistCommand = new BaseCommand<object>(sender => true, SaveBlacklist);
            SaveWhitelistCommand = new BaseCommand<object>(sender => true, SaveWhitelist);
        }

        public PostFilterModel PostFilter
        {
            get => (PostFilterModel) GetValue(PostFilterProperty);
            set => SetValue(PostFilterProperty, value);
        }

        public ICommand SaveBlacklistCommand { get; set; }

        public ICommand SaveWhitelistCommand { get; set; }

        public Visibility CommentVisibility
        {
            get => (Visibility) GetValue(CommentVisibilityProperty);
            set => SetValue(CommentVisibilityProperty, value);
        }

        public void SaveBlacklist(object sender)
        {
            PostFilter.CaptionBlacklists = CaptionBlacklistInputBox.InputText;
            PostFilter.RestrictedPostCaptionList =
                new ObservableCollection<string>(Regex.Split(CaptionBlacklistInputBox.InputText, "\r\n").ToList());
            GlobusLogHelper.log.Info("Caption blacklist saved successfully");
        }

        public void SaveWhitelist(object sender)
        {
            PostFilter.CaptionWhitelist = CaptionWhitelistInputBox.InputText;
            PostFilter.AcceptedPostCaptionList =
                new ObservableCollection<string>(Regex.Split(CaptionWhitelistInputBox.InputText, "\r\n").ToList());
            GlobusLogHelper.log.Info("Caption whitelist saved successfully");
        }
    }
}