#region

using CommonServiceLocator;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;

#endregion

namespace DominatorHouseCore.BusinessLogic.Scheduler
{
    public class DominatorJobProcessFactory : IJobProcessFactory
    {
        public IJobProcess Create(string account, string template, TimingRange currentJobTimeRange, string module,
            SocialNetworks networks)
        {
            var jobProcessHandler = InstanceProvider.GetInstance<IJobProcessFactory>(networks.ToString());
            return jobProcessHandler.Create(account, template, currentJobTimeRange, module, networks);
        }
    }
}