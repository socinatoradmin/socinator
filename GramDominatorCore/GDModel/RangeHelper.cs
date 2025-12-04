using DominatorHouseCore.Utility;
using ProtoBuf;

namespace GramDominatorCore.GDModel
{
    [ProtoContract]
    public class RangeHelper
    {
        //public RangeHelper()
        //{

        //}

        //public RangeHelper(int begin, int end)
        //{
        //    BeginRange = begin;
        //    EndRange = end;
        //}

        [ProtoMember(1)]
        public int BeginRange { get; set; }

        [ProtoMember(2)]
        public int EndRange { get; set; }

      //  public bool IsValidRange => EndRange >= BeginRange;

        public int GetRandom()
        {
            return RandomUtilties.GetRandomNumber(EndRange, BeginRange);
        }

        //public bool InRange(int number)
        //{
        //    if (number >= BeginRange)
        //        return number <= EndRange;
        //    return false;
        //}
    }
}
