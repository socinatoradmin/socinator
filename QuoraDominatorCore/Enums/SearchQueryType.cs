namespace QuoraDominatorCore.Enums
{
    public enum SearchQueryType
    {
        Questions=0,
        Answers=1,
        Posts=2,
        Profiles=3,
        Topics=4,
        Spaces=5
    }
    public enum UserActivityType
    {
        Profile=0,
        Answers=1,
        Questions=2,
        Posts=3,
        Followers=4,
        Followings=5,
        Edits=6,
        Activity=7,
        ProfileFollowers=8,
        ProfileFollowings=9,
        AnswersOfQuestion=10,
        AnswerDetail=11,
        TopicAnswer=12,
        AnswerUpvoter=13,
        QuestionCommentor=14,
        AnswerCommentor=15
    }
    public enum PostQueryType
    {
        Post,
        Answer
    }
    public enum VoteQueryType
    {
        UpVote,
        DownVote
    }
}
