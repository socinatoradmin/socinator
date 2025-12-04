using System;

namespace DominatorHouseCore.Models
{
    public class SubscribtionPopUpModel
    {
        public string Title { get; set; } = "Reminder";
        public string Description { get; set; } = "Your trial has started and valid upto 02/03/2023";
        public string SubDescription { get; set; } = "If you want to cancel the subscription\nchoose option below and place cancellation request.";
        public string StripeDashBoardUrl { get; set; } = "https://dashboard.stripe.com/";
        public bool IsAboutToExpire {  get; set; }
        public DateTime expires {  get; set; }
        public DateTime nextTimeToShow { get; set; }
        public string Key { get; set; }
        public bool IsTrial=false;
        public string PayPalDashBoardUrl { get; set; } = "https://www.paypal.com/myaccount/";
    }
}
