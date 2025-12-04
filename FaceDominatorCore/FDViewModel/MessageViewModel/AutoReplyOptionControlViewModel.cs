using DominatorHouseCore.Command;
using DominatorHouseCore.Models.FacebookModels;
using DominatorHouseCore.Utility;
using System.Windows;
using System.Windows.Input;

namespace FaceDominatorCore.FDViewModel.MessageViewModel
{
    public class AutoReplyOptionControlViewModel : BindableBase
    {
        public ICommand CheckUncheckCommand { get; set; }

        public AutoReplyOptionControlViewModel()
        {

            AutoReplyOptionModel = new AutoReplyOptionModel
            {
                BySoftwareDisplayName = Application.Current.FindResource("LangKeyReplyToNewPendingMessagesReplyToMessageRequests")?.ToString(),
                OutsideSoftwareDisplayName = Application.Current.FindResource("LangKeyReplyToConnectedPeopleReplyToThePeopleWhoAreConnectedInMessanger")?.ToString()
            };

            CheckUncheckCommand = new BaseCommand<object>((sender) => true, CheckUncheckOwnPage);
        }

        private void CheckUncheckOwnPage(object obj)
        {
            if (!AutoReplyOptionModel.IsReplyToPageMessagesChecked)
            {
                AutoReplyOptionModel.OwnPages = string.Empty;
            }
        }

        private AutoReplyOptionModel _AutoReplyOptionModel = new AutoReplyOptionModel();
        public AutoReplyOptionModel AutoReplyOptionModel
        {
            get
            {
                return _AutoReplyOptionModel;
            }
            set
            {
                if (_AutoReplyOptionModel == null & _AutoReplyOptionModel == value)
                    return;
                SetProperty(ref _AutoReplyOptionModel, value);
            }
        }

    }
}
