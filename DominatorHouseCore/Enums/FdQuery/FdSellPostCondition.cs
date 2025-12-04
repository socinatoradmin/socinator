using System.ComponentModel;

namespace DominatorHouseCore.Enums.FdQuery
{
    public enum FdSellPostCondition
    {
        [Description("New")] New = 1,
        [Description("Used – like new")] Usedlikenew = 2,
        [Description("Used – good")] Usedgood = 3,
        [Description("Used – fair")] Usedfair = 4,
    }
}
