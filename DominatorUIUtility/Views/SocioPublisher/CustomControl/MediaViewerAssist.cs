using System;
using System.Collections.ObjectModel;
using System.Windows;
using DominatorHouseCore;

namespace DominatorUIUtility.Views.SocioPublisher.CustomControl
{
    public static class MediaViewerAssist
    {
        #region Post Description

        public static readonly DependencyProperty PostDescriptionProperty = DependencyProperty.RegisterAttached(
            "PostDescription", typeof(string), typeof(MediaViewerAssist),
            new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.Inherits));

        public static void SetPostDescription(DependencyObject element, string value)
        {
            element.SetValue(PostDescriptionProperty, value);
        }

        public static string GetPostDescription(DependencyObject element)
        {
            return (string) element.GetValue(PostDescriptionProperty);
        }

        #endregion

        #region IsEnablePreviousPointer

        public static readonly DependencyProperty IsEnablePreviousPointerProperty = DependencyProperty.RegisterAttached(
            "IsEnablePreviousPointer", typeof(bool), typeof(MediaViewerAssist),
            new FrameworkPropertyMetadata(default(bool), FrameworkPropertyMetadataOptions.Inherits));

        public static void SetIsEnablePreviousPointer(DependencyObject element, bool value)
        {
            element.SetValue(IsEnablePreviousPointerProperty, value);
        }

        public static bool GetIsEnablePreviousPointer(DependencyObject element)
        {
            return (bool) element.GetValue(IsEnablePreviousPointerProperty);
        }

        #endregion

        #region IsEnableNextPointer

        public static readonly DependencyProperty IsEnableNextPointerProperty = DependencyProperty.RegisterAttached(
            "IsEnableNextPointer", typeof(bool), typeof(MediaViewerAssist),
            new FrameworkPropertyMetadata(default(bool), FrameworkPropertyMetadataOptions.Inherits));

        public static void SetIsEnableNextPointer(DependencyObject element, bool value)
        {
            element.SetValue(IsEnableNextPointerProperty, value);
        }

        public static bool GetIsEnableNextPointer(DependencyObject element)
        {
            return (bool) element.GetValue(IsEnableNextPointerProperty);
        }

        #endregion

        #region Media Height

        public static readonly DependencyProperty MediaHeightProperty = DependencyProperty.RegisterAttached(
            "MediaHeight", typeof(double), typeof(MediaViewerAssist),
            new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.Inherits));

        public static void SetMediaHeight(DependencyObject element, double value)
        {
            element.SetValue(MediaHeightProperty, value);
        }

        public static double GetMediaHeight(DependencyObject element)
        {
            return (double) element.GetValue(MediaHeightProperty);
        }

        #endregion

        #region Media Width

        public static readonly DependencyProperty MediaWidthProperty = DependencyProperty.RegisterAttached(
            "MediaWidth", typeof(double), typeof(MediaViewerAssist),
            new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.Inherits));

        public static void SetMediaWidth(DependencyObject element, double value)
        {
            element.SetValue(MediaWidthProperty, value);
        }

        public static double GetMediaWidth(DependencyObject element)
        {
            return (double) element.GetValue(MediaWidthProperty);
        }

        #endregion

        #region Delete Menu Visibility

        public static readonly DependencyProperty DeleteMenuProperty = DependencyProperty.RegisterAttached(
            "DeleteMenu", typeof(Visibility), typeof(MediaViewerAssist),
            new FrameworkPropertyMetadata(default(Visibility), FrameworkPropertyMetadataOptions.Inherits));

        public static void SetDeleteMenu(DependencyObject element, Visibility value)
        {
            element.SetValue(DeleteMenuProperty, value);
        }

        public static Visibility GetDeleteMenu(DependencyObject element)
        {
            return (Visibility) element.GetValue(DeleteMenuProperty);
        }

        #endregion

        #region IsPostDetailsPresent

        public static readonly DependencyProperty IsPostDetailsPresentProperty = DependencyProperty.RegisterAttached(
            "IsPostDetailsPresent", typeof(bool), typeof(MediaViewerAssist),
            new FrameworkPropertyMetadata(default(bool), FrameworkPropertyMetadataOptions.Inherits));

        public static void SetIsPostDetailsPresent(DependencyObject element, bool value)
        {
            element.SetValue(IsPostDetailsPresentProperty, value);
        }

        public static bool GetIsPostDetailsPresent(DependencyObject element)
        {
            return (bool) element.GetValue(IsPostDetailsPresentProperty);
        }

        #endregion

        #region CurrentMediaUrl

        public static readonly DependencyProperty CurrentMediaUrlProperty = DependencyProperty.RegisterAttached(
            "CurrentMediaUrl", typeof(string), typeof(MediaViewerAssist),
            new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.Inherits));

        public static void SetCurrentMediaUrl(DependencyObject element, string value)
        {
            element.SetValue(CurrentMediaUrlProperty, value);
        }

        public static string GetCurrentMediaUrl(DependencyObject element)
        {
            return (string) element.GetValue(CurrentMediaUrlProperty);
        }

        #endregion

        #region TotalMediaCount

        public static readonly DependencyProperty TotalMediaCountProperty = DependencyProperty.RegisterAttached(
            "TotalMediaCount", typeof(int), typeof(MediaViewerAssist),
            new FrameworkPropertyMetadata(default(int), FrameworkPropertyMetadataOptions.Inherits));

        public static void SetTotalMediaCount(DependencyObject element, int value)
        {
            element.SetValue(TotalMediaCountProperty, value);
        }

        public static int GetTotalMediaCount(DependencyObject element)
        {
            return (int) element.GetValue(TotalMediaCountProperty);
        }

        #endregion

        #region Current Media Pointer

        public static readonly DependencyProperty CurrentMediaPointerProperty = DependencyProperty.RegisterAttached(
            "CurrentMediaPointer", typeof(int), typeof(MediaViewerAssist),
            new FrameworkPropertyMetadata(default(int), FrameworkPropertyMetadataOptions.Inherits));

        public static void SetCurrentMediaPointer(DependencyObject element, int value)
        {
            element.SetValue(CurrentMediaPointerProperty, value);
        }

        public static int GetCurrentMediaPointer(DependencyObject element)
        {
            return (int) element.GetValue(CurrentMediaPointerProperty);
        }

        #endregion

        #region MediaList

        public static readonly DependencyProperty MediaListProperty = DependencyProperty.RegisterAttached(
            "MediaList", typeof(ObservableCollection<string>), typeof(MediaViewerAssist),
            new FrameworkPropertyMetadata(new ObservableCollection<string>(), FrameworkPropertyMetadataOptions.Inherits,
                CallBack));

        private static void CallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                //IsPostDataPresent = MediaList.Count > 0;
                //if (IsPostDataPresent)
                //{
                //    ImagePointer = 0;
                //    CurrentMediaPointer = 1;
                //    CurrentMediaUrl = MediaList[ImagePointer];
                //    TotalMediaCount = MediaList.Count;
                //    IsEnableNextPointer = (TotalMediaCount - ImagePointer) > 1;
                //    IsEnablePreviousPointer = ImagePointer > 0;
                //}
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public static void SetMediaList(DependencyObject element, ObservableCollection<string> value)
        {
            element.SetValue(MediaListProperty, value);
        }

        public static ObservableCollection<string> GetMediaList(DependencyObject element)
        {
            return (ObservableCollection<string>) element.GetValue(MediaListProperty);
        }

        #endregion
    }
}