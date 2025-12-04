using System.Collections.Generic;
using System.Threading.Tasks;

namespace DominatorHouseCore.Interfaces
{
    public interface IAccountSyncManager
    {
        Task<bool> AddBrowser(string browserName);
        Task<bool> RemoveBrowser(string browserName);
        Task<bool> RemoveAll();
        Task<int> GetBrowserCount();
    }
    public class AccountSyncManager :List<string>, IAccountSyncManager
    {
        public async Task<bool> AddBrowser(string browserName)
        {
            try
            {
                if (!Contains(browserName))
                {
                    Add(browserName);
                }
                return await Task.FromResult(true);
            }
            catch
            {
                return false;
            }
        }

        public async Task<int> GetBrowserCount()
        {
            return await Task.FromResult(Count);
        }

        public async Task<bool> RemoveAll()
        {
            try
            {
                Clear();
                return await Task.FromResult(true);
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> RemoveBrowser(string browserName)
        {
            try
            {
                Remove(browserName);
                return await Task.FromResult(true);
            }
            catch
            {
                return false;
            }
        }
    }
}
