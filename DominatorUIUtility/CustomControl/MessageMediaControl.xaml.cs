using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    ///     Interaction logic for MessageMediaControl.xaml
    /// </summary>
    public partial class MessageMediaControl
    {
        // Using a DependencyProperty as the backing store for ManageComments.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ManageMessagesProperty =
            DependencyProperty.Register("Messages", typeof(ManageMessagesModel), typeof(MessageMediaControl), new FrameworkPropertyMetadata
            {
                BindsTwoWayByDefault = true
            });

        // Using a DependencyProperty as the backing store for LstManageCommentModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LstManageMessagesModelProperty =
            DependencyProperty.Register("LstManageMessagesModel", typeof(ObservableCollection<ManageMessagesModel>),
                typeof(MessageMediaControl), new PropertyMetadata(new ObservableCollection<ManageMessagesModel>()));

        // Using a DependencyProperty as the backing store for Network.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NetworkProperty =
            DependencyProperty.Register("Network", typeof(SocialNetworks), typeof(MessageMediaControl),
                new PropertyMetadata(SocialNetworks.Social));

        // Using a DependencyProperty as the backing store for AddMessagesCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AddMessagesCommandProperty =
            DependencyProperty.Register("AddMessagesCommand", typeof(ICommand), typeof(MessageMediaControl));

        // Using a DependencyProperty as the backing store for CommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(MessageMediaControl),
                new PropertyMetadata());
        private bool _isUncheckfromList;
        public static readonly DependencyProperty ShowMediaControlProperty =
            DependencyProperty.Register("ShowMediaControl", typeof(Visibility), typeof(MessageMediaControl), new PropertyMetadata
            {
                DefaultValue = Visibility.Collapsed
            });
        public MessageMediaControl()
        {
            InitializeComponent();
            MainGrid.DataContext = this;
            AddMessagesCommand = new BaseCommand<object>(sender => true, AddMessagesExecute);
        }
        public bool IsEnableMultiSelect => ShowMediaControl==Visibility.Visible;
        public Visibility ShowMediaControl
        {
            get => (Visibility)GetValue(ShowMediaControlProperty);
            set=> SetValue(ShowMediaControlProperty, value);
        }
        public bool Isupdated { get; set; }

        public ManageMessagesModel Messages
        {
            get => (ManageMessagesModel) GetValue(ManageMessagesProperty);
            set => SetValue(ManageMessagesProperty, value);
        }

        public ObservableCollection<ManageMessagesModel> LstManageMessagesModel
        {
            get => (ObservableCollection<ManageMessagesModel>) GetValue(LstManageMessagesModelProperty);
            set => SetValue(LstManageMessagesModelProperty, value);
        }

        public SocialNetworks Network
        {
            get => (SocialNetworks) GetValue(NetworkProperty);
            set => SetValue(NetworkProperty, value);
        }

        public ICommand AddMessagesCommand
        {
            get => (ICommand) GetValue(AddMessagesCommandProperty);
            set => SetValue(AddMessagesCommandProperty, value);
        }
        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        private void CheckUncheckAll(object sender, bool IsChecked)
        {
            var dataContext = (sender as CheckBox)?.DataContext;
            if (dataContext is QueryContent content)
            {
                var currentQuery = content.Content.QueryValue;
                if (!Messages.LstQueries.Skip(1).All(x => x.IsContentSelected))
                    if (!IsChecked)
                    {
                        _isUncheckfromList = true;
                        Messages.LstQueries[0].IsContentSelected = false;
                    }

                if (Messages.LstQueries.Skip(1).All(x => x.IsContentSelected))
                {
                    _isUncheckfromList = false;
                    Messages.LstQueries[0].IsContentSelected = IsChecked;
                }

                if (_isUncheckfromList)
                {
                    _isUncheckfromList = false;
                    return;
                }

                if (currentQuery == "All" || currentQuery == "Default")
                {
                    _isUncheckfromList = false;
                    Messages.LstQueries.ToList().ForEach(query =>
                    {
                        query.IsContentSelected = IsChecked;
                    });
                }
            }
        }

        private void AddCheckedQueryToList()
        {
            Messages.SelectedQuery.Clear();
            Messages.LstQueries.ToList().ForEach(query =>
            {
                if (query.IsContentSelected)
                    Messages.SelectedQuery.Add(query);
            });
        }

        private void chkQuery_Checked(object sender, RoutedEventArgs e)
        {
            CheckUncheckAll(sender, true);
        }

        private void chkQuery_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckUncheckAll(sender, false);
        }

        private void btnPhotos_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var opf = new OpenFileDialog();
                opf.Filter =
                    "Image Files |*.jpg;*.jpeg;*.png;*.gif"; //"Image Files(*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF";
                opf.Multiselect = IsEnableMultiSelect;
                if (opf.ShowDialog().Value)
                {
                    Messages.MediaPath = opf.FileName;
                    Messages.Medias.Clear();
                    if (IsEnableMultiSelect)
                    {
                        foreach (var file in opf.FileNames)
                        {
                            Messages.Medias.Add(new MessageMediaInfo(file));
                        }
                    }else
                        Messages.Medias.Add(new MessageMediaInfo(Messages.MediaPath));
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void DeleteMedia_Click(object sender, RoutedEventArgs e)
        {
            Messages.MediaPath = "";
            if (!IsEnableMultiSelect)
                Messages.Medias.Clear();
        }
        private void AddMessagesExecute(object sender)
        {
            if (string.IsNullOrEmpty(Messages.MessagesText) && string.IsNullOrEmpty(Messages.MediaPath))
            {
                Dialog.ShowDialog(Application.Current.MainWindow, "Warning",
                    "Please type some message !!");
                return;
            }

            AddCheckedQueryToList();
            if (Messages.SelectedQuery.Count == 0)
            {
                Dialog.ShowDialog(Application.Current.MainWindow, "Warning",
                    "Please add atleast one query!!");
                return;
            }

            if (!Messages.LstQueries.Any(x => x.IsContentSelected))
            {
                Dialog.ShowDialog("Warning", "Please select atleast one query.");
                return;
            }

            if (btnAddMessagesToList.Content.ToString() == "Update Message")
            {
                LstManageMessagesModel.ForEach(x =>
                {
                    if (x.MessageId == Messages.MessageId)
                    {
                        x.MessagesText = Messages.MessagesText;
                        x.LstQueries = Messages.LstQueries;
                        x.MediaPath = Messages.MediaPath;
                        x.SelectedQuery = Messages.SelectedQuery;
                        x.Medias = Messages.Medias;
                        x.ShowMediaControl = Messages.ShowMediaControl;
                    }
                });
                Messages.SelectedQuery.Remove(
                    Messages.SelectedQuery.FirstOrDefault(x => x.Content.QueryValue == "All"));
                Messages.LstQueries.ForEach(x =>
                {
                    x.IsContentSelected = false;
                });
                Isupdated = true;
                Dialog.CloseDialog(this);
            }
        }
        private void btnVideos_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var opf = new OpenFileDialog();
                opf.Filter = "Video Files |*.mp4;"; //"Image Files(*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF";
                opf.Multiselect = IsEnableMultiSelect;
                if (opf.ShowDialog().Value)
                {
                    Messages.MediaPath = opf.FileName;
                    Messages.Medias.Clear();
                    if (IsEnableMultiSelect)
                    {
                        foreach (var file in opf.FileNames)
                        {
                            Messages.Medias.Add(new MessageMediaInfo(file));
                        }
                    }else
                        Messages.Medias.Add(new MessageMediaInfo(Messages.MediaPath));
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}