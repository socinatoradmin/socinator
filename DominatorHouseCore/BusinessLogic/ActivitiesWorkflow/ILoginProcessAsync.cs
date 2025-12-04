#region

using System.Threading;
using System.Threading.Tasks;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;

#endregion

namespace DominatorHouseCore.BusinessLogic.ActivitiesWorkflow
{
    public interface ILoginProcessAsync : ILoginProcess
    {
        Task<bool> CheckLoginAsync(DominatorAccountModel dominatorAccountModel, CancellationToken cancellationToken,
            bool displayLoginMsg = false, LoginType loginType = LoginType.AutomationLogin);

        Task LoginWithDataBaseCookiesAsync(DominatorAccountModel dominatorAccountModel, bool isMobileRequired,
            CancellationToken cancellationToken);

        Task LoginWithAlternativeMethodAsync(DominatorAccountModel dominatorAccountModel,
            CancellationToken cancellationToken);
    }
}