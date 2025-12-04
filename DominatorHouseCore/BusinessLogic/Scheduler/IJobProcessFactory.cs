#region

using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;

#endregion

namespace DominatorHouseCore.BusinessLogic.Scheduler
{
    /// <summary>
    ///     Each library have to implement its own job process factory to create particular
    ///     activities via scheduler
    /// </summary>
    public interface IJobProcessFactory
    {
        IJobProcess Create(string account, string template, TimingRange currentJobTimeRange, string module,
            SocialNetworks network);
    }
}