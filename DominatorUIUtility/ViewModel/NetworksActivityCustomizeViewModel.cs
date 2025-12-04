using System.Linq;
using System.Windows.Input;
using CommonServiceLocator;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Prism.Commands;

namespace DominatorUIUtility.ViewModel
{
    public class NetworksActivityCustomizeViewModel : BindableBase
    {
        private NetworksActivityCustomizeModel _model;

        public NetworksActivityCustomizeViewModel()
        {
            ChangeActivityStatusCmd = new DelegateCommand<NetworkCustomizeActivityTypeModel>(ChangeActivityStatus);
            SaveCommand = new DelegateCommand(Save);
        }

        public NetworksActivityCustomizeViewModel(NetworksActivityCustomizeModel model) : this()
        {
            Model = model;
        }

        public NetworksActivityCustomizeModel Model
        {
            get => _model;
            set => SetProperty(ref _model, value);
        }

        public ICommand SaveCommand { get; }
        public bool IsSaved { get; set; }
        public DelegateCommand<NetworkCustomizeActivityTypeModel> ChangeActivityStatusCmd { get; }

        private void ChangeActivityStatus(NetworkCustomizeActivityTypeModel currentDataContext)
        {
            var getOne = Model.NetworksActListCollection.ToList()
                .FirstOrDefault(x => x.SocialNetwork == currentDataContext.Network);

            if (currentDataContext.IsSelected)
            {
                if (getOne.NetworkActivityTypeModelCollections.Count(x => x.IsSelected) > 6)
                {
                    var lastOne =
                        getOne.NetworkActivityTypeModelCollections.Last(x =>
                            x.IsSelected && x.Title != currentDataContext.Title);
                    lastOne.IsSelected = false;
                }
            }
            else if (getOne.NetworkActivityTypeModelCollections.Count(x => x.IsSelected) == 0)
            {
                currentDataContext.IsSelected = true;
            }
        }

        private void Save()
        {
            var binFileHelper = InstanceProvider.GetInstance<IBinFileHelper>();
            if (binFileHelper.SaveAutoActivityCustomized(Model))
            {
                ToasterNotification.ShowSuccess("LangKeySucceededInSaving".FromResourceDictionary());
                IsSaved = true;
            }
            else
            {
                ToasterNotification.ShowError("LangKeyOopsAnErrorOccured".FromResourceDictionary());
            }
        }
    }
}