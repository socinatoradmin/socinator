#region

using System.ComponentModel;

#endregion

namespace DominatorHouseCore.Enums.DHEnum
{
    public enum GrowthChartPeriod
    {
        [Description("Past day")] PastDay = 0,

        [Description("Past week")] PastWeek,

        [Description("Past 30 days")] Past30Days,

        [Description("Past 3 Months")] Past3Months,

        [Description("Past 6 Months")] Past6Months,

        [Description("All time")] AllTime
    }

    public enum GrowthChartType
    {
        [Description("LangKeyGain")] Gain = 1,
        [Description("LangKeyTotal")] Total = 2,
        [Description("LangKeyBoth")] Both = 3
    }
}