using DominatorHouseCore.Interfaces;

namespace FaceDominatorCore.FDLibrary.FdClassLibrary
{
    public class FanpageDetails : IPage
    {
        public string FanPageName { get; set; }

        public string FanPageID { get; set; }

        public string FanPageUrl { get; set; }
        public string FanPageProfilePicurl { get; set; }

        public string FanPageLikerCount { get; set; }

        public string FanpageFollowerCount { get; set; }

        public string FanPageCategory { get; set; }

        public string FanPageDescription { get; set; }

        public string IsLikedByFriend { get; set; }

        public bool IsLikedByUser { get; set; }

        public string IsVerifiedPage { get; set; }

        public string RatingValue { get; set; }

        public string RatingCount { get; set; }

        public string FanPageMainCategoryName { get; set; }

        public string PhoneNumber { get; set; }

        public string Address { get; set; }

        public string PriceRange { get; set; }

        public string WebAddresss { get; set; }
        //        public bool IsOwnPage { get; set; }
        //
        public string CreationDate { get; set; }
        public string BusinessHour { get; set; }

        public string NeighbourHood { get; set; }

        public string RatingUrl { get; set; }

        public string MenuUrl { get; set; }

        public string Status { get; set; }

        public string CheckIns { get; set; }

        public bool CanSendMessage { get; set; } = false;

        public string Email { get; set; }

        public string WhatsApp { get; set; }

        public string Twitter { get; set; }

        public string Instagram { get; set; }
    }
}
