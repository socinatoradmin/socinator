#region

using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;

#endregion

namespace DominatorHouseCore.Models
{
    public class ScrapeResultNew
    {
        public IUser ResultUser { get; set; }
        public IPost ResultPost { get; set; }
        public IGroup ResultGroup { get; set; }
        public ActivityType ActivityType { get; set; }
        public QueryInfo QueryInfo { get; set; }
        public bool IsAccountLocked { get; set; }
        public IJob ResultJob { get; set; }
        public ICompany ResultCompany { get; set; }

        public IChannel ResultChannel { get; set; }

        public IPage ResultPage { get; set; }

        public IComments ResultComment { get; set; }
        public IEvent ResultEvent { get; set; }
        public IEntity ResultEntity { get; set; }
        public IPostComment ResultPostComment { get; set; }

        public IHashTag ResultHashTag { get; set; }
    }
}