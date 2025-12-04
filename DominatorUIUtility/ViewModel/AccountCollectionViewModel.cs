using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;

namespace DominatorUIUtility.ViewModel
{
    public interface IAccountCollectionViewModel : IList<DominatorAccountModel>, INotifyCollectionChanged,
        INotifyPropertyChanged
    {
        List<DominatorAccountModel> GetCopySync();
        List<DominatorAccountModel> BySocialNetwork(SocialNetworks networks);
        void AddSync(DominatorAccountModel model);
    }

    public class AccountCollectionViewModel : ObservableCollection<DominatorAccountModel>, IAccountCollectionViewModel
    {
        public static readonly object SyncObject = new object();

        public List<DominatorAccountModel> GetCopySync()
        {
            lock (SyncObject)
            {
                return this.ToList();
            }
        }

        public void AddSync(DominatorAccountModel model)
        {
            lock (SyncObject)
            {
                Add(model);
            }
        }

        public List<DominatorAccountModel> BySocialNetwork(SocialNetworks networks)
        {
            lock (SyncObject)
            {
                if (networks == SocialNetworks.Social) return GetCopySync();
                return this.Where(a => a.AccountBaseModel.AccountNetwork == networks).ToList();
            }
        }
    }
}