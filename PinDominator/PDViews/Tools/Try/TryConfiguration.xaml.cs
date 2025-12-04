using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using PinDominator.CustomControl;
using PinDominatorCore.PDEnums;
using PinDominatorCore.PDModel;
using PinDominatorCore.PDUtility;
using PinDominatorCore.PDViewModel.PinTryCommenter;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace PinDominator.PDViews.Tools.Try
{
    public class TryConfigurationBase : ModuleSettingsUserControl<TryViewModel, TryModel>
    {
        protected override bool ValidateExtraProperty()
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

            if (ObjViewModel.TryModel.ChkTryUsersLatestPinsChecked && ObjViewModel.TryModel.LstNotes.Count == 0)
            {
                Dialog.ShowDialog(this, "Error",
                    "Please add at least one try text in after try action.");
                return false;
            }

            return true;
        }

        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.TryModel =
                        templateModel.ActivitySettings.GetActivityModel<TryModel>(ObjViewModel.TryModel);
                //JsonConvert.DeserializeObject<TryModel>(templateModel.ActivitySettings);
                else if (ObjViewModel == null)
                    ObjViewModel = new TryViewModel();

                ObjViewModel.TryModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }

    /// <summary>
    ///     Interaction logic for LikeConfiguration.xaml
    /// </summary>
    public partial class TryConfiguration
    {
        private readonly QueryContent _queryContent = new QueryContent {Content = new QueryInfo {QueryValue = "All"}};

        private TryConfiguration()
        {
            InitializeComponent();
            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.Try,
                Enums.PdMainModule.TryComment.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: TryConfigurationSearchControl
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.TryVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.TryKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            ObjViewModel.TryModel.ManageNoteModel.LstQueries.Add(_queryContent);
        }

        private static TryConfiguration CurrentTry { get; set; }

        /// <summary>
        ///     GetSingeltonObjectTryConfiguration is used to get the object of the current user control,
        ///     if object is already created then its wont create a new object object, simply it returns already created object,
        ///     otherwise create a new object and then its return.
        /// </summary>
        /// <returns>Current UI class object</returns>
        public static TryConfiguration GetSingeltonObjectTryConfiguration()
        {
            return CurrentTry ?? (CurrentTry = new TryConfiguration());
        }

        private void note_AddNoteToListChanged(object sender, RoutedEventArgs e)
        {
            var noteData = sender as NoteControl;

            if (noteData == null) return;

            noteData.Notes.TryText = noteData.Notes.TryText.Trim();
            if (string.IsNullOrEmpty(noteData.Notes.MediaPath))
            {
                Dialog.ShowDialog(this, "Manage Notes Input Error",
                    "Please upload media");
                return;
            }

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