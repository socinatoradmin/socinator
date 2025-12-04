using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using FaceDominatorCore.FDEnums;
using FaceDominatorCore.FDModel.FbEvents;
using FaceDominatorCore.FDViewModel.FbCreator;
using MahApps.Metro.Controls.Dialogs;

namespace FaceDominatorUI.FDViews.FbEvents
{
    public class EventCreatorBase : ModuleSettingsUserControl<EventCreatorViewModel, EventCreatorModel>
    {
        protected override bool ValidateCampaign()
        {
            if (Model.LstManageEventModel.Count <= 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyAddEventsToList".FromResourceDictionary());
                return false;
            }

            return base.ValidateCampaign();
        }
    }

    public partial class EventCreator
    {
        public EventCreator()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: HeaderGrid,
                footer: EventCreaterFooter,
                queryControl: null,
                MainGrid: MainGrid,
                activityType: ActivityType.EventCreator,
                moduleName: FdMainModule.Events.ToString()
            );
            base.SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        private static EventCreator CurrentEventCreator { get; set; }

        public static EventCreator GetSingeltonObjectEventCreator()
        {
            return CurrentEventCreator ?? (CurrentEventCreator = new EventCreator());
        }
    }
}