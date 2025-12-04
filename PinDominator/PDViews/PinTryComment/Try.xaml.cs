using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using PinDominator.CustomControl;
using PinDominatorCore.PDModel;
using PinDominatorCore.PDUtility;
using PinDominatorCore.PDViewModel.PinTryCommenter;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using static PinDominatorCore.PDEnums.Enums;

namespace PinDominator.PDViews.PinTryComment
{
    public class TryBase : ModuleSettingsUserControl<TryViewModel, TryModel>
    {
        protected override bool ValidateCampaign()
        {
            // Check queries
            if (ObjViewModel.TryModel.SavedQueries.Count == 0)
            {
                Dialog.ShowDialog(this, "Error", "Please add at least one query.");
                return false;
            }

            if (ObjViewModel.TryModel.LstDisplayManageNoteModel.Count == 0)
            {
                Dialog.ShowDialog(this, "Error", "Please add at least one Note.");
                return false;
            }

            if (ObjViewModel.TryModel.ChkTryUsersLatestPinsChecked &&
                (ObjViewModel.TryModel.LstNotes.Count == 0 || string.IsNullOrEmpty(Model.MediaPath)))
            {
                Dialog.ShowDialog(this, "Error",
                    "Please add at least one try text in after try action.");
                return false;
            }

            return base.ValidateCampaign();
        }
    }

    /// <summary>
    ///     Interaction logic for Try.xaml
    /// </summary>
    public sealed partial class Try
    {
        private Try()
        {
            InitializeComponent();

            InitializeBaseClass
            (
                header: TryHeader,
                footer: TryFooter,
                queryControl: TrySearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.Try,
                moduleName: PdMainModule.TryComment.ToString()
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.TryVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.TryKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            SetDataContext();
            DialogParticipation.SetRegister(this, this);
            //ObjViewModel.TryModel.ManageNoteModel.LstQueries.Add(QueryContent);
        }

        private static Try CurrentTry { get; set; }

        /// <summary>
        ///     GetSingeltonObjectTry is used to get the object of the current user control,
        ///     if object is already created then its wont create a new object object, simply it returns already created object,
        ///     otherwise create a new object and then its return.
        /// </summary>
        /// <returns>Current UI class object</returns>
        public static Try GetSingeltonObjectTry()
        {
            return CurrentTry ?? (CurrentTry = new Try());
        }

        private void NoteControl_AddNoteToListChanged(object sender, RoutedEventArgs e)
        {
            var noteData = sender as NoteControl;

            if (noteData == null) return;

            if (string.IsNullOrEmpty(noteData.Notes.MediaPath))
            {
                Dialog.ShowDialog(this, "Manage Notes Input Error",
                    "Please upload media");
                return;
            }

            noteData.Notes.TryText = noteData.Notes.TryText.Trim();
            noteData.Notes.SelectedQuery =
                new ObservableCollection<QueryContent>(noteData.Notes.LstQueries.Where(x => x.IsContentSelected));

            if (noteData.Notes.SelectedQuery.Count == 0 || string.IsNullOrEmpty(noteData.Notes.TryText))
            {
                Dialog.ShowDialog(this, "Warning", "Please type some note !!");
                return;
            }

            noteData.Notes.SelectedQuery.Remove(
                noteData.Notes.SelectedQuery.FirstOrDefault(x => x.Content.QueryValue == "All"));

            Model.LstDisplayManageNoteModel.Add(noteData.Notes);

            noteData.Notes = new ManageNoteModel
            {
                LstQueries = Model.ManageNoteModel.LstQueries
            };

            noteData.Notes.LstQueries.ForEach(query => { query.IsContentSelected = false; });

            Model.ManageNoteModel = noteData.Notes;
            noteData.ComboBoxQueries.ItemsSource = Model.ManageNoteModel.LstQueries;
        }

        private void DeleteMedia_Click(object sender, RoutedEventArgs e)
        {
            Model.MediaPath = string.Empty;
        }
    }
}