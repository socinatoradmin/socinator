using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using System;

namespace TumblrDominatorCore.TumblrFactory
{
    public class TrdJobProcessFactory : IJobProcessFactory
    {
        private static TrdJobProcessFactory _instance;

        public static TrdJobProcessFactory Instance => _instance ?? (_instance = new TrdJobProcessFactory());

        public IJobProcess Create(string account, string template, TimingRange currentJobTimeRange, string module,
            SocialNetworks network)
        {
            throw new NotImplementedException();
        }
    }
}