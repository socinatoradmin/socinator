using System;
using System.Windows;
using DominatorHouseCore;
using DominatorHouseCore.Models.FacebookModels;
using DominatorHouseCore.Utility;
using Microsoft.Win32;

namespace DominatorUIUtility.CustomControl.FacebookCustomControl
{
    /// <summary>
    ///     Interaction logic for FbMultiMediaControl.xaml
    /// </summary>
    public partial class FbMultiMediaControl
    {
        // Using a DependencyProperty as the backing store for FbMultiMediaModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FbMultiMediaModelProperty =
            DependencyProperty.Register("FbMultiMediaModel", typeof(FbMultiMediaModel),
                typeof(FbMultiMediaControl), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
                {
                    BindsTwoWayByDefault = true
                });

        // Using a DependencyProperty as the backing store for ActivityType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ActivityTypeProperty =
            DependencyProperty.Register("ActivityType", typeof(string),
                typeof(FbMultiMediaControl), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
                {
                    BindsTwoWayByDefault = true
                });

        // Using a DependencyProperty as the backing store for MediaHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MediaHeightProperty =
            DependencyProperty.Register("MediaHeight", typeof(double),
                typeof(FbMultiMediaControl), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
                {
                    BindsTwoWayByDefault = true
                });

        // Using a DependencyProperty as the backing store for MediaHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CloseButtonwidthProperty =
            DependencyProperty.Register("CloseButtonwidth", typeof(double),
                typeof(FbMultiMediaControl), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
                {
                    BindsTwoWayByDefault = true
                });

        public FbMultiMediaControl()
        {
            InitializeComponent();
            MainGrid.DataContext = this;
        }

        public FbMultiMediaModel FbMultiMediaModel
        {
            get => (FbMultiMediaModel) GetValue(FbMultiMediaModelProperty);
            set => SetValue(FbMultiMediaModelProperty, value);
        }


        public string ActivityType
        {
            get => (string) GetValue(ActivityTypeProperty);
            set => SetValue(ActivityTypeProperty, value);
        }

        public double MediaHeight
        {
            get => (double) GetValue(MediaHeightProperty);
            set => SetValue(MediaHeightProperty, value);
        }


        public double CloseButtonwidth
        {
            get => (double) GetValue(CloseButtonwidthProperty);
            set => SetValue(CloseButtonwidthProperty, value);
        }

        public static void OnAvailableItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var imageValue = ((FrameworkElement) sender).DataContext as MultiMediaValueModel;
            FbMultiMediaModel.MediaPaths.Remove(imageValue);
            FbMultiMediaModel.IsAddImageVisibile = true;
        }

        private void DeleteMedia_Click(object sender, RoutedEventArgs e)
        {
            var imageValue = ((FrameworkElement) sender).DataContext as MultiMediaValueModel;
            FbMultiMediaModel.MediaPaths.Remove(imageValue);
            FbMultiMediaModel.IsAddImageVisibile = true;
        }

        private void BtnPhotos_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var openFileDialog = new OpenFileDialog
                {
                    Multiselect = FbMultiMediaModel.IsMultiselect,
                    Filter =
                        "Image Files |*.jpg;*.jpeg;*.png;*.gif|Videos Files |*.dat; *.wmv; *.3g2; *.3gp; *.3gp2; *.3gpp; *.amv; *.asf;  *.avi; *.bin; *.cue; *.divx; *.dv; *.flv; *.gxf; *.iso; *.m1v; *.m2v; *.m2t; *.m2ts; *.m4v; " +
                        " *.mkv; *.mov; *.mp2; *.mp2v; *.mp4; *.mp4v; *.mpa; *.mpe; *.mpeg; *.mpeg1; *.mpeg2; *.mpeg4; *.mpg; *.mpv2; *.mts; *.nsv; *.nuv; *.ogg; *.ogm; *.ogv; *.ogx; *.ps; *.rec; *.rm; *.rmvb; *.tod; *.ts; *.tts; *.vob; *.vro; *.webm"
                };
                if (openFileDialog.ShowDialog() ?? false)
                    foreach (var fileName in openFileDialog.FileNames)
                        FbMultiMediaModel.MediaPaths.Add(new MultiMediaValueModel
                        {
                            MediaHeight = MediaHeight,
                            MediaPath = fileName
                        });

                if (FbMultiMediaModel.MediaPaths.Count != 0
                    && ActivityType == "LangKeyEventCreater".FromResourceDictionary())
                    FbMultiMediaModel.IsAddImageVisibile = false;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}