using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using PinDominatorCore.PDModel;

namespace PinDominator.CustomControl
{
    /// <summary>
    ///     Interaction logic for NoteControl.xaml
    /// </summary>
    public partial class NoteControl
    {
        private static readonly RoutedEvent AddNoteToListEvent =
            EventManager.RegisterRoutedEvent("AddNoteToListChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler),
                typeof(NoteControl));

        // Using a DependencyProperty as the backing store for ManageNotes.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ManageNotesProperty =
            DependencyProperty.Register("Notes", typeof(ManageNoteModel), typeof(NoteControl),
                new PropertyMetadata());

        // Using a DependencyProperty as the backing store for LstManageCommentModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LstManageNoteModelProperty =
            DependencyProperty.Register("LstManageNoteModel", typeof(ObservableCollection<ManageNoteModel>),
                typeof(NoteControl), new PropertyMetadata(new ObservableCollection<ManageNoteModel>()));

        // Using a DependencyProperty as the backing store for AddCommentsCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AddNotesCommandProperty =
            DependencyProperty.Register("AddNotesCommand", typeof(ICommand), typeof(NoteControl));

        // Using a DependencyProperty as the backing store for CommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(NoteControl),
                new PropertyMetadata());

        public NoteControl()
        {
            InitializeComponent();
            MainGrid.DataContext = this;
            AddNotesCommand = new BaseCommand<object>(sender => true, AddNotesExecute);
        }

        public bool Isupdated { get; set; }

        public ManageNoteModel Notes
        {
            get => (ManageNoteModel) GetValue(ManageNotesProperty);
            set => SetValue(ManageNotesProperty, value);
        }

        public ObservableCollection<ManageNoteModel> LstManageNoteModel
        {
            get => (ObservableCollection<ManageNoteModel>) GetValue(LstManageNoteModelProperty);
            set => SetValue(LstManageNoteModelProperty, value);
        }

        public ICommand AddNotesCommand
        {
            get => (ICommand) GetValue(AddNotesCommandProperty);
            set => SetValue(AddNotesCommandProperty, value);
        }

        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        public event RoutedEventHandler AddNoteToListChanged
        {
            add => AddHandler(AddNoteToListEvent, value);
            remove => RemoveHandler(AddNoteToListEvent, value);
        }

        private void AddNoteToListEventHandler()
        {
            var routedEventArgs = new RoutedEventArgs(AddNoteToListEvent);
            RaiseEvent(routedEventArgs);
        }

        private void btnAddNoteToList_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Notes.TryText))
            {
                Dialog.ShowDialog(Application.Current.MainWindow, "Warning",
                    "Please type some Note !!");
                return;
            }

            AddCheckedQueryToList();
            if (BtnAddNoteToList.Content.ToString() == "Update Note")
            {
                LstManageNoteModel.ForEach(x =>
                {
                    if (x.SerialNo == Notes.SerialNo)
                    {
                        x.TryText = Notes.TryText;
                        x.FilterText = Notes.FilterText;
                        x.LstQueries = Notes.LstQueries;
                    }
                });
                Notes.SelectedQuery.Remove(Notes.SelectedQuery.FirstOrDefault(x => x.Content.QueryValue == "All"));
                Notes.LstQueries.ForEach(x => { x.IsContentSelected = false; });
                Window.GetWindow(this)?.Close();
            }
            else
            {
                AddNoteToListEventHandler();
            }
        }

        private void chkQuery_Checked(object sender, RoutedEventArgs e)
        {
            CheckUncheckAll(sender, true);
        }

        private void chkQuery_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckUncheckAll(sender, false);
        }

        private void CheckUncheckAll(object sender, bool isChecked)
        {
            if (((QueryContent) (sender as CheckBox)?.DataContext)?.Content.QueryValue == "All")
                Notes.LstQueries.ToList().ForEach(query => { query.IsContentSelected = isChecked; });
        }

        private void AddCheckedQueryToList()
        {
            Notes.SelectedQuery.Clear();
            Notes.LstQueries.ToList().ForEach(query =>
            {
                if (query.IsContentSelected)
                    Notes.SelectedQuery.Add(query);
            });
        }

        private void AddNotesExecute(object sender)
        {
            if (string.IsNullOrEmpty(Notes.TryText))
            {
                Dialog.ShowDialog(Application.Current.MainWindow, "Warning",
                    "Please type some note !!");
                return;
            }

            AddCheckedQueryToList();
            if (BtnAddNoteToList.Content.ToString() == "Update Note")
            {
                LstManageNoteModel.ForEach(x =>
                {
                    if (x.SerialNo == Notes.SerialNo)
                    {
                        x.TryText = Notes.TryText;
                        x.FilterText = Notes.FilterText;
                        x.LstQueries = Notes.LstQueries;
                    }
                });
                Notes.SelectedQuery.Remove(Notes.SelectedQuery.FirstOrDefault(x => x.Content.QueryValue == "All"));
                Notes.LstQueries.ForEach(x => { x.IsContentSelected = false; });
                Isupdated = true;
                Dialog.CloseDialog(this);
            }
            else
            {
                AddNoteToListEventHandler();
            }
        }

        private void btnPhotos_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var opf = new OpenFileDialog {Filter = "Image Files(*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF"};
                if (opf.ShowDialog() ?? false) Notes.MediaPath = opf.FileName;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void DeleteMedia_Click(object sender, RoutedEventArgs e)
        {
            Notes.MediaPath = "";
        }
    }
}