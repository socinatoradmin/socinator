using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.FdQuery;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Models.SocioPublisher.Settings;
using DominatorHouseCore.Patterns;
using DominatorHouseCore.Utility;
using DominatorUIUtility.ViewModel.SocioPublisher;
using DominatorUIUtility.Views.SocioPublisher.CustomControl.Settings;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;

namespace DominatorUIUtility.Views.SocioPublisher.CustomControl
{
    /// <summary>
    ///     Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///     Step 1a) Using this custom control in a XAML file that exists in the current project.
    ///     Add this XmlNamespace attribute to the root element of the markup file where it is
    ///     to be used:
    ///     xmlns:MyNamespace="clr-namespace:DominatorUIUtility.Views.SocioPublisher.CustomControl"
    ///     Step 1b) Using this custom control in a XAML file that exists in a different project.
    ///     Add this XmlNamespace attribute to the root element of the markup file where it is
    ///     to be used:
    ///     xmlns:MyNamespace="clr-namespace:DominatorUIUtility.Views.SocioPublisher.CustomControl;assembly=DominatorUIUtility.Views.SocioPublisher.CustomControl"
    ///     You will also need to add a project reference from the project where the XAML file lives
    ///     to this project and Rebuild to avoid compilation errors:
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///     Step 2)
    ///     Go ahead and use your control in the XAML file.
    ///     <MyNamespace:PostContent />
    /// </summary>
    [TemplatePart(Type = typeof(Button), Name = ButtonImportImage)]
    [TemplatePart(Type = typeof(Button), Name = ButtonSettings)]
    [TemplatePart(Type = typeof(MediaViewer), Name = MediaViewerControl)]
    public class PostContent : Control
    {
        static PostContent()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PostContent),
                new FrameworkPropertyMetadata(typeof(PostContent)));
        }

        #region Properties

        public const string ButtonImportImage = "PART_ImportImage";
        public const string ButtonSettings = "PART_Settings";
        public const string MediaViewerControl = "PART_MediaViewer";
        public const string ImportPostTitle = "PART_ImportPostTitle";
        public const string ClearPostTitle = "PART_ClearPostTitle";

        public string PostDescription
        {
            get => (string)GetValue(PostDescriptionProperty);
            set => SetValue(PostDescriptionProperty, value);
        }

        // Using a DependencyProperty as the backing store for PostDescription.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PostDescriptionProperty =
            DependencyProperty.Register("PostDescription", typeof(string), typeof(PostContent),
                new PropertyMetadata(string.Empty));

        public string PublisherInstagramTitle
        {
            get => (string)GetValue(PublisherInstagramTitleProperty);
            set => SetValue(PublisherInstagramTitleProperty, value);
        }

        public static readonly DependencyProperty PublisherInstagramTitleProperty =
            DependencyProperty.Register("PublisherInstagramTitle", typeof(string), typeof(PostContent),
                new PropertyMetadata(string.Empty));


        public string FdSellProductTitle
        {
            get => (string)GetValue(FdSellProductTitleProperty);
            set => SetValue(FdSellProductTitleProperty, value);
        }

        // Using a DependencyProperty as the backing store for FdSellProductTitle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FdSellProductTitleProperty =
            DependencyProperty.Register("FdSellProductTitle", typeof(string), typeof(PostContent),
                new PropertyMetadata(string.Empty));


        public double FdSellPrice
        {
            get => (double)GetValue(FdSellPriceProperty);
            set => SetValue(FdSellPriceProperty, value);
        }

        // Using a DependencyProperty as the backing store for FdSellPrice.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FdSellPriceProperty =
            DependencyProperty.Register("FdSellPrice", typeof(double), typeof(PostContent),
                new PropertyMetadata(double.MinValue));


        public string FdSellLocation
        {
            get => (string)GetValue(FdSellLocationProperty);
            set => SetValue(FdSellLocationProperty, value);
        }

        // Using a DependencyProperty as the backing store for FdSellLocation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FdSellLocationProperty =
            DependencyProperty.Register("FdSellLocation", typeof(string), typeof(PostContent),
                new PropertyMetadata(string.Empty));

        public List<string> FdConditionItems
        {
            get => (List<string>)GetValue(FdConditionItemsProperty);
            set => SetValue(FdConditionItemsProperty, value);
        }
        public static readonly DependencyProperty FdConditionItemsProperty =
            DependencyProperty.Register("FdConditionItems", typeof(List<string>), typeof(PostContent),
                new PropertyMetadata(Enum.GetValues(typeof(FdSellPostCondition)).Cast<FdSellPostCondition>().ToList().Select(x => x.GetDescriptionAttr()).ToList()));
        public static void OnAvailableItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
        }
        public string FdCondition
        {
            get => (string)GetValue(FdConditionProperty);
            set => SetValue(FdConditionProperty, value);
        }
        // Using a DependencyProperty as the backing store for FdCondition.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FdConditionProperty =
            DependencyProperty.Register("FdCondition", typeof(string), typeof(PostContent),
                new PropertyMetadata(string.Empty));
        public string PdSourceUrl
        {
            get => (string)GetValue(PdSourceUrlProperty);
            set => SetValue(PdSourceUrlProperty, value);
        }

        // Using a DependencyProperty as the backing store for PdSourceUrl.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PdSourceUrlProperty =
            DependencyProperty.Register("PdSourceUrl", typeof(string), typeof(PostContent),
                new PropertyMetadata(string.Empty));


        public Visibility IsImportOptionsVisibility
        {
            get => (Visibility)GetValue(IsImportOptionsVisibilityProperty);
            set => SetValue(IsImportOptionsVisibilityProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsImportOptionsVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsImportOptionsVisibilityProperty =
            DependencyProperty.Register("IsImportOptionsVisibility", typeof(Visibility), typeof(PostContent),
                new PropertyMetadata(Visibility.Visible));


        public bool IsFdSellPost
        {
            get => (bool)GetValue(IsFdSellPostProperty);
            set => SetValue(IsFdSellPostProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsFdSellPost.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsFdSellPostProperty =
            DependencyProperty.Register("IsFdSellPost", typeof(bool), typeof(PostContent), new PropertyMetadata(false));

        public bool IsChangeHashOfMedia
        {
            get => (bool)GetValue(IsChangeHashOfMediaProperty);
            set => SetValue(IsChangeHashOfMediaProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsChangeHashOfMedia.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsChangeHashOfMediaProperty =
            DependencyProperty.Register("IsChangeHashOfMedia", typeof(bool), typeof(PostContent),
                new PropertyMetadata(false));


        private Button _selectMedia = new Button();
        private Button _buttonSettings = new Button();
        private Button _buttonImportPostTitle = new Button();
        private Button _buttonClearPostTitle = new Button();


        public PublisherPostSettings PublisherPostSettings
        {
            get => (PublisherPostSettings)GetValue(PublisherPostSettingsProperty);
            set => SetValue(PublisherPostSettingsProperty, value);
        }

        // Using a DependencyProperty as the backing store for PublisherPostSettings.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PublisherPostSettingsProperty =
            DependencyProperty.Register("PublisherPostSettings", typeof(PublisherPostSettings), typeof(PostContent),
                new PropertyMetadata(new PublisherPostSettings()));

        public bool IsRandomlyPickTitleFromList
        {
            get => (bool)GetValue(IsRandomlyPickTitleFromListProperty);
            set => SetValue(IsRandomlyPickTitleFromListProperty, value);
        }

        public static readonly DependencyProperty IsRandomlyPickTitleFromListProperty =
            DependencyProperty.Register("IsRandomlyPickTitleFromList", typeof(bool), typeof(PostContent),
                new PropertyMetadata(false));

        public bool IsRemoveTitleOnceUsed
        {
            get => (bool)GetValue(IsRemoveTitleOnceUsedProperty);
            set => SetValue(IsRemoveTitleOnceUsedProperty, value);
        }

        public static readonly DependencyProperty IsRemoveTitleOnceUsedProperty =
            DependencyProperty.Register("IsRemoveTitleOnceUsed", typeof(bool), typeof(PostContent),
                new PropertyMetadata(false));

        public Visibility IsPostTitleOptionVisibility
        {
            get => (Visibility)GetValue(IsPostTitleOptionVisibilityProperty);
            set => SetValue(IsPostTitleOptionVisibilityProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsImportOptionsVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsPostTitleOptionVisibilityProperty =
            DependencyProperty.Register("IsPostTitleOptionVisibility", typeof(Visibility), typeof(PostContent),
                new PropertyMetadata(Visibility.Collapsed));

        public double PostTitleHeight
        {
            get => (double)GetValue(PostTitleHeightProperty);
            set => SetValue(PostTitleHeightProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsImportOptionsVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PostTitleHeightProperty =
            DependencyProperty.Register("PostTitleHeight", typeof(double), typeof(PostContent),
                new PropertyMetadata(30.0));

        public Visibility IsImportPostTitleOptionVisibility
        {
            get => (Visibility)GetValue(IsImportPostTitleOptionVisibilityProperty);
            set => SetValue(IsImportPostTitleOptionVisibilityProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsImportOptionsVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsImportPostTitleOptionVisibilityProperty =
            DependencyProperty.Register("IsImportPostTitleOptionVisibility", typeof(Visibility), typeof(PostContent),
                new PropertyMetadata(Visibility.Collapsed));

        public Visibility IsClearPostTitleOptionVisibility
        {
            get => (Visibility)GetValue(IsClearPostTitleOptionVisibilityProperty);
            set => SetValue(IsClearPostTitleOptionVisibilityProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsImportOptionsVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsClearPostTitleOptionVisibilityProperty =
            DependencyProperty.Register("IsClearPostTitleOptionVisibility", typeof(Visibility), typeof(PostContent),
                new PropertyMetadata(Visibility.Collapsed));

        public Visibility IsMediaVisibility
        {
            get => (Visibility)GetValue(IsMediaVisibilityProperty);
            set => SetValue(IsMediaVisibilityProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsImportOptionsVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsMediaVisibilityProperty =
            DependencyProperty.Register("IsMediaVisibility", typeof(Visibility), typeof(PostContent),
                new PropertyMetadata(Visibility.Visible));

        public Visibility IsSourceUrlAndFdSellPostVisible
        {
            get => (Visibility)GetValue(IsSourceUrlAndFdSellPostVisibleProperty);
            set => SetValue(IsSourceUrlAndFdSellPostVisibleProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsImportOptionsVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsSourceUrlAndFdSellPostVisibleProperty =
            DependencyProperty.Register("IsSourceUrlAndFdSellPostVisible", typeof(Visibility), typeof(PostContent),
                new PropertyMetadata(Visibility.Visible));

        public bool IsSpinTax
        {
            get => (bool)GetValue(IsSpinTaxProperty);
            set => SetValue(IsSpinTaxProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsFdSellPost.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsSpinTaxProperty =
            DependencyProperty.Register("IsSpinTax", typeof(bool), typeof(PostContent), new PropertyMetadata(false));

        #endregion

        #region Apply Template

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            #region Button Import Images

            var buttonMedia = Template.FindName(ButtonImportImage, this) as Button;

            if (!_selectMedia.Equals(buttonMedia))
            {
                if (buttonMedia != null)
                    buttonMedia.Click -= SelectMediaClick;

                _selectMedia = buttonMedia;

                if (buttonMedia != null)
                    buttonMedia.Click += SelectMediaClick;
            }

            #endregion

            #region Button Import Post title

            var importPostTitle = Template.FindName(ImportPostTitle, this) as Button;

            if (!_buttonImportPostTitle.Equals(importPostTitle))
            {
                if (importPostTitle != null)
                    importPostTitle.Click -= ImportPostTitleClick;

                _buttonImportPostTitle = importPostTitle;

                if (importPostTitle != null)
                    importPostTitle.Click += ImportPostTitleClick;
            }

            #endregion

            #region Button Clear Post title

            var clearPostTitle = Template.FindName(ClearPostTitle, this) as Button;

            if (!_buttonClearPostTitle.Equals(clearPostTitle))
            {
                if (clearPostTitle != null)
                    clearPostTitle.Click -= clearPostTitleClick;

                _buttonClearPostTitle = importPostTitle;

                if (clearPostTitle != null)
                    clearPostTitle.Click += clearPostTitleClick;
            }

            #endregion

            #region Settings 

            var buttonSettingChanges = Template.FindName(ButtonSettings, this) as Button;

            if (!_buttonSettings.Equals(buttonSettingChanges))
            {
                if (buttonSettingChanges != null) buttonSettingChanges.Click -= PostSettingsChangeClick;
                _buttonSettings = buttonSettingChanges;
                if (buttonSettingChanges != null) buttonSettingChanges.Click += PostSettingsChangeClick;
            }

            #endregion

            Loaded += PostContentLoad;
        }

        private void clearPostTitleClick(object sender, RoutedEventArgs e)
        {
            PublisherInstagramTitle = string.Empty;
            tempList = new List<string>();
        }

        private List<string> tempList = new List<string>();

        private void ImportPostTitleClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var lstPostTitle = FileUtilities.FileBrowseAndReader();
                if (lstPostTitle.Count != 0)
                {
                    lstPostTitle.ForEach(title =>
                    {
                        if (!tempList.Any(x => x == title))
                            tempList.Add(title);
                    });
                    PublisherInstagramTitle = string.Join("\n", tempList.ToArray());
                    tempList.Clear();
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void PostContentLoad(object sender, RoutedEventArgs e)
        {
            SetMedia();
        }

        #endregion

        #region Routed Events

        public static readonly RoutedEvent SelectMediaEvent = EventManager.RegisterRoutedEvent(
            "SelectMediaHandler",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(PostContent));

        public event RoutedEventHandler SelectMediaHandler
        {
            add => AddHandler(SelectMediaEvent, value);
            remove => RemoveHandler(SelectMediaEvent, value);
        }

        private void SelectMedia()
        {
            var args = new RoutedEventArgs(SelectMediaEvent);
            RaiseEvent(args);
        }


        public static readonly RoutedEvent PostSettings = EventManager.RegisterRoutedEvent("PostSettingHandler",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(PostContent));

        public event RoutedEventHandler PostSettingHandler
        {
            add => AddHandler(PostSettings, value);
            remove => RemoveHandler(PostSettings, value);
        }

        private void PostSettingEvent()
        {
            var args = new RoutedEventArgs(PostSettings);
            RaiseEvent(args);
        }

        #endregion

        #region Events

        public void PostSettingsChangeClick(object sender, RoutedEventArgs args)
        {
            var postSettingsDeepClone = PublisherPostSettings.DeepClone();

            var objAdvancedSettings = new PostAdvancedSettings(PublisherPostSettings);

            var dialogWindow = Dialog.Instance.GetMetroWindowWithOutClose(objAdvancedSettings, "Post Settings");

            objAdvancedSettings.ButtonSave.Click += (senders, events) =>
            {
                try
                {
                    if (!Validation.GetHasError(objAdvancedSettings.PostGeneralSettings.DatePicker))
                        dialogWindow.Close();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            };

            objAdvancedSettings.ButtonCancel.Click += (senders, events) =>
            {
                objAdvancedSettings.PostGeneralSettings.PublisherPostSettings = postSettingsDeepClone;

                var availableNetworks = SocinatorInitialize.AvailableNetworks;

                foreach (var network in availableNetworks)
                    switch (network)
                    {
                        case SocialNetworks.Facebook:
                            objAdvancedSettings.PostFacebookSettings.PublisherPostSettings = postSettingsDeepClone;
                            break;
                        case SocialNetworks.Instagram:
                            objAdvancedSettings.PostInstagramSettings.PublisherPostSettings = postSettingsDeepClone;
                            break;
                        case SocialNetworks.Twitter:
                            objAdvancedSettings.PostTwitterSettings.PublisherPostSettings = postSettingsDeepClone;
                            break;
                        case SocialNetworks.LinkedIn:
                            objAdvancedSettings.PostLinkedInSettings.PublisherPostSettings = postSettingsDeepClone;
                            break;
                        case SocialNetworks.Tumblr:
                            objAdvancedSettings.PostTumblrSettings.PublisherPostSettings = postSettingsDeepClone;
                            break;
                        case SocialNetworks.Reddit:
                            objAdvancedSettings.PostRedditSettings.PublisherPostSettings = postSettingsDeepClone;
                            break;
                        case SocialNetworks.Pinterest:
                        case SocialNetworks.Quora:
                        //case SocialNetworks.Gplus:
                        case SocialNetworks.YouTube:
                            break;
                        // ReSharper disable once RedundantEmptySwitchSection
                        default:
                            break;
                    }

                PublisherPostSettings = postSettingsDeepClone;
                dialogWindow.Close();
            };

            dialogWindow.ShowDialog();

            PostSettingEvent();
        }

        public void SelectMediaClick(object sender, RoutedEventArgs args)
        {
            var postSettingsDeepClone = PublisherPostSettings.DeepClone();
            var openFileDialog = new OpenFileDialog();
            //post doc,pdf file for only linkedin
            if (postSettingsDeepClone.LdPostSettings.IsDocTypePosts)
            {
                openFileDialog = new OpenFileDialog
                {
                    Multiselect = true,
                    Filter =
                     "All Files|*.*|Image Files |*.jpg;*.jpeg;*.png;*.gif|Videos Files |*.dat; *.wmv; *.3g2; *.3gp; *.3gp2; *.3gpp; *.amv; *.asf;  *.avi; *.bin; *.cue; *.divx; *.dv; *.flv; *.gxf; *.iso; *.m1v; *.m2v; *.m2t; *.m2ts; *.m4v; " +
                     " *.mkv; *.mov; *.mp2; *.mp2v; *.mp4; *.mp4v; *.mpa; *.mpe; *.mpeg; *.mpeg1; *.mpeg2; *.mpeg4; *.mpg; *.mpv2; *.mts; *.nsv; *.nuv; *.ogg; *.ogm; *.ogv; *.ogx; *.ps; *.rec; *.rm; *.rmvb; *.tod; *.ts; *.tts; *.vob; *.vro; *.webm"
                };
            }
            else
            {
                openFileDialog = new OpenFileDialog
                {
                    Multiselect = true,
                    Filter =
                    "Image Files |*.jpg;*.jpeg;*.png;*.gif|Videos Files |*.dat; *.wmv; *.3g2; *.3gp; *.3gp2; *.3gpp; *.amv; *.asf;  *.avi; *.bin; *.cue; *.divx; *.dv; *.flv; *.gxf; *.iso; *.m1v; *.m2v; *.m2t; *.m2ts; *.m4v; " +
                    " *.mkv; *.mov; *.mp2; *.mp2v; *.mp4; *.mp4v; *.mpa; *.mpe; *.mpeg; *.mpeg1; *.mpeg2; *.mpeg4; *.mpg; *.mpv2; *.mts; *.nsv; *.nuv; *.ogg; *.ogm; *.ogv; *.ogx; *.ps; *.rec; *.rm; *.rmvb; *.tod; *.ts; *.tts; *.vob; *.vro; *.webm"
                };
            }

            var openFileDialogResult = openFileDialog.ShowDialog();
            if (openFileDialogResult != true)
                return;

            var files = openFileDialog.FileNames.ToList();

            var mediaViewer = Template.FindName(MediaViewerControl, this) as MediaViewer;

            if (mediaViewer != null)
            {
                var mediaUtilites = new MediaUtilites();
                if (mediaViewer.MediaList == null)
                    mediaViewer.MediaList = new ObservableCollection<string>();
                files.ForEach(x =>
                {
                    MediaViewerAssist.SetMediaList(this, mediaViewer.MediaList);
                    mediaUtilites.GetThumbnail(x);
                    mediaViewer.MediaList.Add(x);
                    //MediaViewer.MediaList.Add(x);
                });
                mediaViewer.Initialize();
            }

            // Raise your event
            SelectMedia();
        }

        internal void SetMedia()
        {
            var mediaViewer = Template.FindName(MediaViewerControl, this) as MediaViewer;
            try
            {
                if (mediaViewer != null)
                {
                    var dataContext = mediaViewer.DataContext;
                    mediaViewer.MediaList = (dataContext as PostDetailsModel)?.MediaViewer.MediaList;
                    if (dataContext is PublisherRssFeedViewModel)
                        mediaViewer.MediaList = mediaViewer.MediaList ??
                                                (mediaViewer.DataContext as PublisherRssFeedViewModel)
                                                ?.PublisherRssFeedModel.PostDetailsModel.MediaList;
                    else if (dataContext is PublisherDirectPostsViewModel)
                        mediaViewer.MediaList = mediaViewer.MediaList ??
                                                (mediaViewer.DataContext as PublisherDirectPostsViewModel)
                                                ?.PostDetailsModel.MediaViewer.MediaList;
                    else if (dataContext is PublisherPostlistModel)
                        mediaViewer.MediaList = (dataContext as PublisherPostlistModel)?.MediaList;
                    if (mediaViewer.MediaList == null)
                        mediaViewer.MediaList = new ObservableCollection<string>();
                    mediaViewer.Initialize();
                }
            }
            catch (Exception)
            {
                if (mediaViewer != null)
                    mediaViewer.Initialize();
            }
        }

        #endregion
    }
}