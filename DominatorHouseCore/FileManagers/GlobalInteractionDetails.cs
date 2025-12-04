#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorHouseCore.ViewModel;

#endregion

namespace DominatorHouseCore.FileManagers
{
    public class GlobalInteractionDetails : IGlobalInteractionDetails
    {
        private readonly IBinFileHelper _binFileHelper;

        private readonly IReadOnlyDictionary<SocialNetworks, Lazy<Dictionary<ActivityType, GlobalInteractionDataModel>>>
            _globalInteractedCollections;

        private readonly IReadOnlyDictionary<SocialNetworks, object> _networkLocks;

        public GlobalInteractionDetails(IBinFileHelper binFileHelper)
        {
            _binFileHelper = binFileHelper;
            _globalInteractedCollections = Enum.GetValues(typeof(SocialNetworks)).Cast<SocialNetworks>().ToDictionary(
                a => a, a => new Lazy<Dictionary<ActivityType, GlobalInteractionDataModel>>(
                    () =>
                    {
                        var saveData = _binFileHelper.GetGlobalInteractedDetails(a);
                        if (saveData?.Count > 0)
                            return saveData[0].GlobalInteractedCollections;

                        return new Dictionary<ActivityType, GlobalInteractionDataModel>();
                    }, LazyThreadSafetyMode.ExecutionAndPublication));
            _networkLocks = Enum.GetValues(typeof(SocialNetworks)).Cast<SocialNetworks>()
                .ToDictionary(a => a, a => new object());
        }

        public GlobalInteractionDataModel this[SocialNetworks networks, ActivityType activityType]
        {
            get
            {
                lock (_networkLocks[networks])
                {
                    var interactedData = _globalInteractedCollections[networks].Value;
                    if (interactedData.ContainsKey(activityType)) return interactedData[activityType];

                    return null;
                }
            }
        }

        public void AddInteractedData(SocialNetworks networks, ActivityType activityType, string interactedData)
        {
            lock (_networkLocks[networks])
            {
                var hashsetValue = new SortedList<string, DateTime> {{interactedData, DateTime.Now}};
                var collections = _globalInteractedCollections[networks].Value;

                if (collections.ContainsKey(activityType))
                    collections[activityType].InteractedData.Add(interactedData, DateTime.Now);
                else
                    collections.Add(activityType,
                        new GlobalInteractionDataModel
                        {
                            InteractedData = hashsetValue
                        });

                UpdateInteractedData(networks);
            }
        }

        public void RemoveIfExist(SocialNetworks networks, ActivityType activityType, string interactedData)
        {
            lock (_networkLocks[networks])
            {
                var model = this[networks, activityType];
                if (model?.InteractedData?.ContainsKey(interactedData) ?? false)
                {
                    model.InteractedData.Remove(interactedData);
                    UpdateInteractedData(networks);
                }
            }
        }

        private void UpdateInteractedData(SocialNetworks networks)
        {
            // Save all the TdGlobalInteractionDetails.GlobalInteractedCollections datas to bin file
            var globalInteractionViewModel = new GlobalInteractionViewModel
            {
                GlobalInteractedCollections = _globalInteractedCollections[networks].Value
            };

            var globalInteractedDatas = new List<GlobalInteractionViewModel> {globalInteractionViewModel};

            _binFileHelper.UpdateGlobalInteractedDetails(globalInteractedDatas, networks);
        }

        public List<string> GetInteractedData(SocialNetworks networks, ActivityType activityType)
        {
            lock (_networkLocks[networks])
            {
                var lstOfUser = new List<string>();
                var collections = _globalInteractedCollections[networks].Value;

                if (collections.ContainsKey(activityType))
                    collections[activityType].InteractedData.ForEach(x => lstOfUser.Add(x.Key.ToString()));
                return lstOfUser;
            }
        }
    }
}