using DominatorHouseCore.Command;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.Views.SocioPublisher;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for ManageMediaUserControl.xaml
    /// </summary>
    public partial class ManageMediaUserControl
    {
        public static Window MediaWindow;
        public static readonly DependencyProperty OpenMediaListProperty =
           DependencyProperty.Register("OpenMediaListCommand", typeof(ICommand), typeof(ManageMediaUserControl));
        public static readonly DependencyProperty MediaListProperty =
           DependencyProperty.Register("Medias", typeof(ObservableCollection<MessageMediaInfo>), typeof(ManageMediaUserControl)
               , new FrameworkPropertyMetadata(OnMediasChanged));
        public static readonly DependencyProperty MediaCountProperty = 
            DependencyProperty.Register(nameof(MediaCount),typeof(int),typeof(ManageMediaUserControl),
                new PropertyMetadata(0));
        public static readonly DependencyProperty ShowMediaControlProperty =
            DependencyProperty.Register("ShowMediaControl", typeof(Visibility), typeof(ManageMediaUserControl),new PropertyMetadata
            {
                DefaultValue = Visibility.Visible
            });
        public int MediaCount
        {
            get => (int)GetValue(MediaCountProperty);
            set => SetValue(MediaCountProperty, value);
        }
        public Visibility ShowMediaControl
        {
            get=>(Visibility)GetValue(ShowMediaControlProperty);
            set=>SetValue(ShowMediaControlProperty,value);
        }
        public ManageMediaUserControl()
        {
            InitializeComponent();
            OpenMediaListCommand = new BaseCommand<object>(sender => true, OpenMedaiListExecute);
        }
        public ObservableCollection<MessageMediaInfo> Medias
        {
            get => (ObservableCollection<MessageMediaInfo>)GetValue(MediaListProperty);
            set { 
                SetValue(MediaListProperty, value);
            }
        }
        private static void OnMediasChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (ManageMediaUserControl)d;

            if (e.OldValue is ObservableCollection<MessageMediaInfo> oldList)
                oldList.CollectionChanged -= control.OnMediasCollectionChanged;

            if (e.NewValue is ObservableCollection<MessageMediaInfo> newList)
            {
                newList.CollectionChanged += control.OnMediasCollectionChanged;
                control.MediaCount = newList.Count;
            }
            else
            {
                control.MediaCount = 0;
            }
        }

        private void OnMediasCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            MediaCount = Medias?.Count ?? 0;
        }
        private void OpenMedaiListExecute(object obj)
        {
            Application.Current.Dispatcher.InvokeAsync(() =>
            {
                try
                {
                    if (Medias.Count > 0)
                    {
                        var MediaControlWindow = MultipleImagePostDialog.GetMultipleImagePostInstance(Medias);
                        var dialog = new Dialog();
                        MediaWindow = dialog.GetMetroWindow(MediaControlWindow, "LangKeyMediaS".FromResourceDictionary());
                        MediaWindow.Closing += (s, e) =>
                        {
                            e.Cancel = true;
                            Medias = MediaControlWindow.ImagePostViewModel.MediaList;
                            MediaWindow.Visibility = Visibility.Collapsed;
                        };
                        MediaWindow.ShowDialog();
                    }
                }
                catch (Exception) { }
            });
        }

        public ICommand OpenMediaListCommand
        {
            get => (ICommand)GetValue(OpenMediaListProperty);
            set => SetValue(OpenMediaListProperty, value);
        }
        private void OpenMediaListWindow(object sender, RoutedEventArgs e)
        {
            OpenMediaListCommand.Execute(sender);
        }
    }
}
