#region

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.DHEnum;
using DominatorHouseCore.Models;
using DominatorHouseCore.ViewModel;

#endregion

namespace DominatorHouseCore.Interfaces
{
    public interface IAccountUpdateFactory
    {
        bool CheckStatus(DominatorAccountModel accountModel);

        bool SolveCaptchaManually(DominatorAccountModel accountModel);

        void UpdateDetails(DominatorAccountModel accountModel);

        DailyStatisticsViewModel GetDailyGrowth(string accuntId, string username, GrowthPeriod period);
        List<DailyStatisticsViewModel> GetDailyGrowthForAccount(string accountId, GrowthChartPeriod period);
    }

    public interface IAccountUpdateFactoryAsync : IAccountUpdateFactory
    {
        Task<bool> CheckStatusAsync(DominatorAccountModel accountModel, CancellationToken token);

        Task UpdateDetailsAsync(DominatorAccountModel accountModel, CancellationToken token);
    }


    public interface IAccountUpdateAccountTypeFactoryAsync : IAccountUpdateFactory
    {
        Task SwitchToBusinessAccountAsync(DominatorAccountModel account, CancellationToken token,
            bool isBusinessAccount = true);

        void SwitchToBusinessAccount(DominatorAccountModel account, CancellationToken token,
            bool isBusinessAccount = true);
    }

    public interface IAccountVerificationFactory
    {
        Task<bool> VerifyAccountAsync(DominatorAccountModel accountModel, VerificationType verificationType,
            CancellationToken token);

        Task<bool> SendVerificationCode(DominatorAccountModel accountModel, VerificationType verificationType,
            CancellationToken token);

        Task<bool> AutoVerifyByEmail(DominatorAccountModel accountModel, CancellationToken token);
    }

    public abstract class ProfileFactory
    {
        public virtual void EditProfile(DominatorAccountModel accountModel)
        {
        }

        public virtual void RemovePhoneVerification(DominatorAccountModel accountModel)
        {
        }
    }
}