#region

using System.Threading;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;

#endregion

namespace DominatorHouseCore.BusinessLogic.ActivitiesWorkflow
{
    /// <summary>
    ///     Interface for log-in for any social network.
    /// </summary>
    public interface ILoginProcess
    {
        bool CheckLogin(DominatorAccountModel dominatorAccountModel, CancellationToken cancellationToken);

        void LoginWithDataBaseCookies(DominatorAccountModel dominatorAccountModel, bool isMobileRequired,
            CancellationToken cancellationToken);

        void LoginWithAlternativeMethod(DominatorAccountModel dominatorAccountModel,
            CancellationToken cancellationToken);

        void LoginWithBrowserMethod(DominatorAccountModel dominatorAccountModel, CancellationToken cancellationToken,
            VerificationType verificationType = 0, LoginType loginType = LoginType.AutomationLogin);
    }
}