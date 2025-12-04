using FluentScheduler;

namespace PinDominatorCore.PDUtility
{
    public class JobScheduler
    {
        public Registry Registry;

        private JobScheduler()
        {
            Registry = new Registry();
            Registry.NonReentrantAsDefault();
            JobManager.Initialize(Registry);
        }
    }
}