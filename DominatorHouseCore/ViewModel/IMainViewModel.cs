#region

using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

#endregion

namespace DominatorHouseCore.ViewModel
{
    public interface IMainViewModel : IDisposable
    {
        void AddNetwork(SocialNetworks socialNetwork);
        void SetActiveNetwork(SocialNetworks social);
        AccessorStrategies Strategies { get; }
        SubscribtionPopUpModel PopUpModel { get; set; }
        ObservableCollection<DominatorAccountModel> AccountList { get; set; }

        KeyValuePair<int, int> ScreenResolution { get; set; }
        void InstallRequiredDependencies();
    }
}