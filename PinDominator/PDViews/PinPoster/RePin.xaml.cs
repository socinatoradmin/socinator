using System;
using System.Linq;
using System.Windows;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using PinDominator.CustomControl;
using PinDominatorCore.PDModel;
using PinDominatorCore.PDUtility;
using PinDominatorCore.PDViewModel.PinPoster;
using static PinDominatorCore.PDEnums.Enums;

namespace PinDominator.PDViews.PinPoster
{
    public class RePinBase : ModuleSettingsUserControl<RePinViewModel, RePinModel>
    {
        protected override bool ValidateExtraProperty()
        {
            // Check queries
            if (ObjViewModel.RePinModel.SavedQueries.Count == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyErrorAddAtLeastOneQuery".FromResourceDictionary());
                return false;
            }

            if (ObjViewModel.RePinModel.AccountPagesBoardsPair == null ||
                ObjViewModel.RePinModel.AccountPagesBoardsPair.All(x => x.IsSelected == false))
            {
                Dialog.ShowDialog(this, "Error", "Please select at least one board.");
                return false;
            }

            if (ObjViewModel.RePinModel.ChkCommentOnPinAfterRepinChecked &&
                ObjViewModel.RePinModel.LstComments.Count == 0)
            {
                Dialog.ShowDialog(this, "Error",
                    "Please add at least one comment in after repin action.");
                return false;
            }

            if (ObjViewModel.RePinModel.ChkTryOnPinAfterRepinChecked && ObjViewModel.RePinModel.LstNotes.Count == 0)
            {
                Dialog.ShowDialog(this, "Error",
                    "Please add at least one try text in after repin action.");
                return false;
            }

            return base.ValidateCampaign();
        }
    }

    /// <summary>
    ///     Interaction logic for RePin﻿.xaml
    /// </summary>
    public sealed partial class RePin
    {
        private Window _window;

        private RePin()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: RePinHeaderControl,
                footer: RePinFooterControl,
                queryControl: RePinSearchQueryControl,
                MainGrid: MainGrid,
                activityType: ActivityType.Repin,
                moduleName: PdMainModule.Poster.ToString()
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.RePinVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.RePinKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        public BoardCreateDestination BoardCreateDestination { get; set; }

        /// <summary>
        ///     This method set DataContext of RePin﻿ model
        /// </summary>
        private static RePin CurrentRePin﻿ { get; set; }

        /// <summary>
        ///     GetSingletonObjectRePin﻿ is used to get the object of the current user control,
        ///     if object is already created then its wont create a new object object, simply it returns already created object,
        ///     otherwise create a new object and then its return.
        /// </summary>
        /// <returns>Current UI class object</returns>
        public static RePin GetSingletonObjectRePin﻿()
        {
            return CurrentRePin﻿ ?? (CurrentRePin﻿ = new RePin());
        }

        public override void SelectAccount()
        {
            try
            {
                BoardCreateDestination = new BoardCreateDestination(ObjViewModel);
                BoardCreateDestination.InitializeProperties();
                BoardCreateDestination.BtnSave.Click += BtnSaveEvent;

                BoardCreateDestination.BoardCreateDestinationsViewModel.PublisherCreateDestinationModel =
                    new BoardCreateDestinationModel();
                _window = new Dialog().GetMetroWindow(BoardCreateDestination, "Select Boards");
                _window.ShowDialog();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void BtnSaveEvent(object sender, EventArgs e)
        {
            try
            {
                BoardCreateDestination.BtnSaveEvent(sender, e);
                var listOfSelectedAccounts = BoardCreateDestination.BoardCreateDestinationsViewModel
                    .PublisherCreateDestinationModel
                    .ListSelectDestination
                    .Where(x => x.IsAccountSelected && x.SocialNetworks == SocialNetworks.Pinterest)
                    .Select(x => x.AccountName).ToList();
                FooterControl_OnSelectAccountChanged(listOfSelectedAccounts);
                _window.Close();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }


        private void DeleteMedia_Click(object sender, RoutedEventArgs e)
        {
            Model.MediaPath = string.Empty;
        }
    }
}