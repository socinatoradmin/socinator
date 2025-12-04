using DominatorHouseCore;
using DominatorHouseCore.Utility;
using PinDominatorCore.PDModel;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PinDominator.CustomControl
{
    /// <summary>
    ///     Interaction logic for ManageNote.xaml
    /// </summary>
    public partial class ManageNote
    {
        // Using a DependencyProperty as the backing store for LstManageNoteModelProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LstManageNoteModelProperty =
            DependencyProperty.Register("LstManageNoteModel", typeof(ObservableCollection<ManageNoteModel>),
                typeof(ManageNote), new PropertyMetadata());

        public ManageNote()
        {
            InitializeComponent();
            MainGrid.DataContext = this;
        }

        public ObservableCollection<ManageNoteModel> LstManageNoteModel
        {
            get => (ObservableCollection<ManageNoteModel>) GetValue(LstManageNoteModelProperty);
            set => SetValue(LstManageNoteModelProperty, value);
        }

        private void DeleteSingleNote_Click(object sender, RoutedEventArgs e)
        {
            var currentItem = ((FrameworkElement) sender).DataContext as ManageNoteModel;
            LstManageNoteModel.Remove(currentItem);
        }

        private void BtnAction_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var contextMenu = ((Button) sender).ContextMenu;
                if (contextMenu != null)
                {
                    contextMenu.DataContext = ((Button) sender).DataContext;
                    contextMenu.IsOpen = true;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void EditNote_Click(object sender, RoutedEventArgs e)
        {
            var currentItem = ((FrameworkElement) sender).DataContext as ManageNoteModel;
            var editNote = new NoteControl {BtnAddNoteToList = {Content = "Update Note"}, Notes = currentItem};

            editNote.Notes?.LstQueries.ToList().ForEach(x =>
            {
                if (editNote.Notes.SelectedQuery.Contains(x))
                    x.IsContentSelected = true;
            });
            editNote.MainGrid.Margin = new Thickness(20);
            var dialog = new Dialog();
            var window = dialog.GetMetroWindow(editNote, "Edit Note");
            window.Show();
        }
    }
}