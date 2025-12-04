using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Models.LinkedinModel;
using DominatorHouseCore.Utility;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace DominatorUIUtility.CustomControl.LinkedinCustomControl
{
    /// <summary>
    /// Interaction logic for ConnectionRequestPersonalNoteControl.xaml
    /// </summary>
    public partial class ConnectionRequestPersonalNoteControl
    {
        private static readonly RoutedEvent AddPersonalNoteToListEvent =
            EventManager.RegisterRoutedEvent("AddPersonalNoteToListChanged", RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(ConnectionRequestPersonalNoteControl));

        // Using a DependencyProperty as the backing store for ManagePersonalNotes.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ManagePersonalNotesProperty =
            DependencyProperty.Register("PersonalNote", typeof(ManagePersonalNoteModel), typeof(ConnectionRequestPersonalNoteControl),
                new PropertyMetadata());

        // Using a DependencyProperty as the backing store for LstManagePersonalNoteModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LstManagePersonalNoteModelProperty =
            DependencyProperty.Register("LstManagePersonalNoteModel", typeof(ObservableCollection<ManagePersonalNoteModel>),
                typeof(ConnectionRequestPersonalNoteControl), new PropertyMetadata(new ObservableCollection<ManagePersonalNoteModel>()));

        // Using a DependencyProperty as the backing store for AddPersonalNotesCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AddPersonalNotesCommandProperty =
            DependencyProperty.Register("AddPersonalNoteCommand", typeof(ICommand), typeof(ConnectionRequestPersonalNoteControl));

        // Using a DependencyProperty as the backing store for CommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(ConnectionRequestPersonalNoteControl),
                new PropertyMetadata());

        private bool _isUncheckfromList;

        public ConnectionRequestPersonalNoteControl()
        {
            InitializeComponent();
            MainGrid.DataContext = this;
            AddPersonalNoteCommand = new BaseCommand<object>(sender => true, AddPersonalNoteExecute);
        }


        public ManagePersonalNoteModel PersonalNote
        {
            get => (ManagePersonalNoteModel)GetValue(ManagePersonalNotesProperty);
            set => SetValue(ManagePersonalNotesProperty, value);
        }

        public ObservableCollection<ManagePersonalNoteModel> LstManagePersonalNoteModel
        {
            get => (ObservableCollection<ManagePersonalNoteModel>)GetValue(LstManagePersonalNoteModelProperty);
            set => SetValue(LstManagePersonalNoteModelProperty, value);
        }

        public bool Isupdated { get; set; }

        public ICommand AddPersonalNoteCommand
        {
            get => (ICommand)GetValue(AddPersonalNotesCommandProperty);
            set => SetValue(AddPersonalNotesCommandProperty, value);
        }

        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        public event RoutedEventHandler AddPersonalNoteToListChanged
        {
            add => AddHandler(AddPersonalNoteToListEvent, value);
            remove => RemoveHandler(AddPersonalNoteToListEvent, value);
        }

        private void AddPersonalNoteToListEventHandler()
        {
            var routedEventArgs = new RoutedEventArgs(AddPersonalNoteToListEvent);
            RaiseEvent(routedEventArgs);
        }

        private void chkQuery_Checked(object sender, RoutedEventArgs e)
        {
            CheckUncheckAll(sender, true);
        }

        private void chkQuery_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckUncheckAll(sender, false);
        }

        private void CheckUncheckAll(object sender, bool IsChecked)
        {
            try
            {
                //var currentQuery = ((QueryContent)(sender as CheckBox)?.DataContext).Content.QueryValue;
                //if (!PersonalNote.LstQueries.Skip(1).All(x => x.IsContentSelected))
                //    if (!IsChecked)
                //    {
                //        _isUncheckfromList = true;
                //        PersonalNote.LstQueries[0].IsContentSelected = false;
                //    }

                //if (PersonalNote.LstQueries.Skip(1).All(x => x.IsContentSelected))
                //{
                //    _isUncheckfromList = false;
                //    PersonalNote.LstQueries[0].IsContentSelected = IsChecked;
                //}

                if (_isUncheckfromList)
                {
                    _isUncheckfromList = false;
                    return;
                }

                //if (currentQuery == "All" || currentQuery == "Default")
                //{
                //    _isUncheckfromList = false;
                //    //PersonalNote.LstQueries.ToList().ForEach(query =>
                //    //{
                //    //    query.IsContentSelected = IsChecked;
                //    //});
                //}
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void AddCheckedQueryToList()
        {
            //PersonalNote.SelectedQuery.Clear();
            //PersonalNote.LstQueries.ToList().ForEach(query =>
            //{
            //    if (query.IsContentSelected)
            //        PersonalNote.SelectedQuery.Add(query);
            //});
        }

        private void AddPersonalNoteExecute(object sender)
        {
            if (string.IsNullOrEmpty(PersonalNote.PersonalNoteText))
            {
                Dialog.ShowDialog("Warning", "Please type some PersonalNote !!");
                return;
            }
            AddCheckedQueryToList();
            if (btnAddPersonalNoteToList.Content.ToString() == "Update PersonalNote")
            {
                LstManagePersonalNoteModel.ForEach(x =>
                {
                    if (x.PersonalNoteId == PersonalNote.PersonalNoteId)
                    {
                        x.PersonalNoteText = PersonalNote.PersonalNoteText;
                    }
                });
                Isupdated = true;
                Dialog.CloseDialog(this);
            }
            else
            {
                AddPersonalNoteToListEventHandler();
            }
        }
    }
}
