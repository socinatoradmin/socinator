#region

using System.Threading;
using System.Threading.Tasks;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;

#endregion

namespace DominatorHouseCore.Interfaces
{
    public interface IBrowserManager
    {
        bool BrowserLogin(DominatorAccountModel account, CancellationToken cancellationToken,
            LoginType loginType = LoginType.AutomationLogin, VerificationType verificationType = 0);
    }
    public interface IBrowserManagerAsync
    {
        Task<bool> BrowserLoginAsync(DominatorAccountModel account, CancellationToken cancellationToken,
            LoginType loginType = LoginType.AutomationLogin, VerificationType verificationType = 0);
    }
}