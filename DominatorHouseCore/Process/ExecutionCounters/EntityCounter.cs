namespace DominatorHouseCore.Process.ExecutionCounters
{
    public class EntityCounter
    {
        public int NoOfActionPerformedCurrentWeek { get; private set; }
        public int NoOfActionPerformedCurrentDay { get; private set; }
        public int NoOfActionPerformedCurrentHour { get; private set; }

        public EntityCounter(int noOfActionPerformedCurrentWeek, int noOfActionPerformedCurrentDay,
            int noOfActionPerformedCurrentHour)
        {
            NoOfActionPerformedCurrentWeek = noOfActionPerformedCurrentWeek;
            NoOfActionPerformedCurrentDay = noOfActionPerformedCurrentDay;
            NoOfActionPerformedCurrentHour = noOfActionPerformedCurrentHour;
        }

        public void Increment()
        {
            NoOfActionPerformedCurrentWeek++;
            NoOfActionPerformedCurrentDay++;
            NoOfActionPerformedCurrentHour++;
        }
    }
}