using DominatorHouseCore.Utility;

namespace QuoraDominatorCore.Interface
{
    public interface IQuestionFilter
    {
        // Filter the Questions by Answer Count that lies between the specified range
        bool FilterAnswersCount { get; set; }
        RangeUtilities AnswersCount { get; set; }


        //Filter the QuestionScraperViewModel by its followers and it lies between the specified range
        bool FilterQuestionFollowers { get; set; }
        RangeUtilities QuestionFollowersCount { get; set; }

        //Filter the QuestionScraperViewModel by Comments count that lies between the specified range
        bool FilterCommentsCount { get; set; }
        RangeUtilities CommentsCount { get; set; }

        //Filter the QuestionScraperViewModel by Public Followers count that lies between the specified range
        RangeUtilities PublicFollowersCount { get; set; }

        //Filter the QuestionScraperViewModel by Views Count that lies between the specified range
        bool FilterViewsCount { get; set; }
        RangeUtilities ViewsCount { get; set; }

        //Filter the QuestionScraperViewModel by Last Asked Date that lies between the specified range
        bool FilterAskedInSpacesCount { get; set; }
        RangeUtilities AskedInSpacesCount { get; set; }

        //Filter the QuestionScraperViewModel by locked status
        bool FilterLockedStatus { get; set; }
    }
}