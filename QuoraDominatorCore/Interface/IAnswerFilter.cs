using DominatorHouseCore.Utility;

namespace QuoraDominatorCore.Interface
{
    public interface IAnswerFilter
    {
        // Filter the answers by views count that lies between specified range
        bool FilterViewsCount { get; }
        RangeUtilities ViewsCount { get; }

        //Filter the answers by up vote count that lies between specified range
        bool FilterUpvoteCounts { get; }
        RangeUtilities UpvoteCount { get; }

        //Filter the answers by answered date
        bool FilterAnsweredDays { get; }
        RangeUtilities AnsweredInLastDays { get; }
    }
}