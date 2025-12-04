using System;
using System.Threading;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using TwtDominatorCore.TDModels;

namespace TwtDominatorCore.TDLibrary.GeneralLibrary.Subprocesses
{
    public interface ISubprocess<in T>
    {
        void Run(CancellationTokenSource cancellationTokenSource, TagDetails tagDetails, T model);
    }

    public abstract class TdSubprocess<T> : ISubprocess<T>
    {
        private readonly ActivityType _activityType;
        private readonly DominatorAccountModel _dominatorAccountModel;
        protected readonly Func<T, int> GetAfterActionDelay;
        protected readonly Func<T, int> GetAfterActionRange;

        protected TdSubprocess(IProcessScopeModel processScopeModel, Func<T, int> getAfterActionRange,
            Func<T, int> getAfterActionDelay)
        {
            GetAfterActionRange = getAfterActionRange;
            GetAfterActionDelay = getAfterActionDelay;
            _dominatorAccountModel = processScopeModel.Account;
            _activityType = processScopeModel.ActivityType;
        }


        public void Run(CancellationTokenSource cancellationTokenSource, TagDetails tagDetails, T model)
        {
            try
            {
                var maxCount = GetAfterActionRange(model);
                if (maxCount == 0) return;

                InternalRun(cancellationTokenSource, tagDetails, model, maxCount);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                ex.DebugLog(
                    $"TwtDominator : [Account: {_dominatorAccountModel.AccountBaseModel.UserName}] => Error : {ex.Message}. StackTrace => {ex.StackTrace}  (Module => {_activityType.ToString()})");
            }
        }

        protected abstract void InternalRun(CancellationTokenSource cancellationTokenSource, TagDetails tagDetails,
            T model, int maxCount);
    }
}