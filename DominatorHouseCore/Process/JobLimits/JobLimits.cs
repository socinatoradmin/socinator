namespace DominatorHouseCore.Process.JobLimits
{
    public class JobLimits
    {
        public int MaxNoOfActionPerWeek { get; }

        public int MaxNoOfActionPerDay { get; }

        public int MaxNoOfActionPerHour { get; }

        public int MaxNoOfActionPerJob { get; }

        public JobLimits(int maxNoOfActionPerWeek, int maxNoOfActionPerDay, int maxNoOfActionPerHour,
            int maxNoOfActionPerJob)
        {
            MaxNoOfActionPerWeek = maxNoOfActionPerWeek;
            MaxNoOfActionPerDay = maxNoOfActionPerDay;
            MaxNoOfActionPerHour = maxNoOfActionPerHour;
            MaxNoOfActionPerJob = maxNoOfActionPerJob;
        }
    }
}