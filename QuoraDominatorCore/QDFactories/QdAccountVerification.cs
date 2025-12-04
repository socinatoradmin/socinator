using System.Threading;
using System.Threading.Tasks;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;

namespace QuoraDominatorCore.QDFactories
{
    public class QdAccountVerification : IAccountVerificationFactory
    {
        public async Task<bool> AutoVerifyByEmail(DominatorAccountModel accountModel, CancellationToken token)
        {
            return true;
        }

        public async Task<bool> SendVerificationCode(DominatorAccountModel accountModel,
            VerificationType verificationType, CancellationToken token)
        {
            return true;
        }

        public async Task<bool> VerifyAccountAsync(DominatorAccountModel accountModel,
            VerificationType verificationType, CancellationToken token)
        {
            return true;
        }
    }
}