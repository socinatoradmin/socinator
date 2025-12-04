using DominatorHouseCore;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.LinkedinModel;
using DominatorHouseCore.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DominatorUIUtility.CustomControl.LinkedinCustomControl
{
    /// <summary>
    /// Interaction logic for ManagePersonalNote.xaml
    /// </summary>
    public partial class ManagePersonalNote
    {
        // Using a DependencyProperty as the backing store for LstManageCommentModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LstManageMessagesModelProperty =
            DependencyProperty.Register("LstManagePersonalNoteModel", typeof(ObservableCollection<ManagePersonalNoteModel>),
                typeof(ManagePersonalNote), new PropertyMetadata());

        public ManagePersonalNote()
        {
            InitializeComponent();
            MainGrid.DataContext = this;
        }

        public ObservableCollection<ManagePersonalNoteModel> LstManagePersonalNoteModel
        {
            get => (ObservableCollection<ManagePersonalNoteModel>)GetValue(LstManageMessagesModelProperty);
            set => SetValue(LstManageMessagesModelProperty, value);
        }

        private void BtnAction_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var contextMenu = ((Button)sender).ContextMenu;
                if (contextMenu != null)
                {
                    contextMenu.DataContext = ((Button)sender).DataContext;
                    contextMenu.IsOpen = true;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void EditPersonalNote_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!(((FrameworkElement)sender).DataContext is ManagePersonalNoteModel currentItem))
                    return;

                var editPersonalNote = new ConnectionRequestPersonalNoteControl
                {
                    btnAddPersonalNoteToList = { Content = "Update PersonalNote" },
                    PersonalNote = new ManagePersonalNoteModel
                    {
                        PersonalNoteText = currentItem.PersonalNoteText,
                        PersonalNoteId = currentItem.PersonalNoteId,
                    },
                    LstManagePersonalNoteModel = LstManagePersonalNoteModel
                };

                editPersonalNote.MainGrid.Margin = new Thickness(20);
                var dialog = new Dialog();
                var window = dialog.GetMetroWindow(editPersonalNote, "Edit Note");
                window.Closed += (s, evnt) =>
                {
                    if (editPersonalNote.Isupdated)
                    {
                        var indexToUpdate = LstManagePersonalNoteModel.IndexOf(currentItem);
                        LstManagePersonalNoteModel[indexToUpdate] = editPersonalNote.PersonalNote;
                    }
                };
                window.ShowDialog();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void DeleteSinglePersonalNote_Click(object sender, RoutedEventArgs e)
        {
            var currentItem = ((FrameworkElement)sender).DataContext as ManagePersonalNoteModel;
            LstManagePersonalNoteModel.Remove(currentItem);
        }
    }
}
