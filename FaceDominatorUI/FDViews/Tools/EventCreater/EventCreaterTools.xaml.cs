using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using FaceDominatorCore.FDEnums;
using FaceDominatorCore.FDModel.FbEvents;
using FaceDominatorCore.FDViewModel.FbCreator;
using FaceDominatorCore.Utility;
using MahApps.Metro.Controls.Dialogs;
using System;

namespace FaceDominatorUI.FDViews.Tools.EventCreater
{
    /// <summary>
    ///     Interaction logic for EventCreaterTools.xaml
    /// </summary>
    public class EventCreatorToolsBase : ModuleSettingsUserControl<EventCreatorViewModel, EventCreatorModel>
    {
        protected override bool ValidateExtraProperty()
        {
            if (Model.LstManageEventModel.Count <= 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyAddEventsToList".FromResourceDictionary());
                return false;
            }

            return true;
        }

        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                {
                    ObjViewModel.EventCreatorModel =
                        templateModel.ActivitySettings.GetActivityModelNonQueryList<EventCreatorModel>(ObjViewModel
                            .Model);
                    ;
                }
                else
                {
                    ObjViewModel = new EventCreatorViewModel();
                }

                ObjViewModel.EventCreatorModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }

    public partial class EventCreaterTools
    {
        private static EventCreaterTools _eventCreaterTools;

        public EventCreaterTools()
        {
            InitializeComponent();
            DialogParticipation.SetRegister(this, this);
            InitializeBaseClass
            (
                MainGrid,
                ActivityType.EventCreator,
                FdMainModule.Events.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader
            );
        }

        public static EventCreaterTools GetSingeltonObjectEventCreaterTools()
        {
            return _eventCreaterTools ?? (_eventCreaterTools = new EventCreaterTools());
        }
    }
}