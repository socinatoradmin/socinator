using DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting;
using DominatorUIUtility.Views.Publisher.AdvancedSettings;
using DominatorUIUtility.Views.SocioPublisher;

namespace DominatorUIUtility.Behaviours
{
    public class AdvanceSetting
    {
        public string CampaignId = PublisherCreateCampaigns.GetSingeltonPublisherCreateCampaigns()
            .PublisherCreateCampaignViewModel
            .PublisherCreateCampaignModel.CampaignId;

        public AdvanceSetting()
        {
            GeneralModel = new GeneralModel();
            GeneralModel.InitializeGeneralModel();
        }

        public FacebookModel FacebookModel { get; set; } =
            Facebook.GetSingeltonFacebookObject().FacebookViewModel.FacebookModel.Clone();

        public GeneralModel GeneralModel { get; set; }

        //  public GooglePlusModel GooglePlusModel { get; set; } = GooglePlus.GetSingeltonGooglePlusObject().GooglePlusViewModel.GooglePlusModel.Clone();

        public InstagramModel InstagramModel { get; set; } =
            Instagram.GetSingeltonInstagramObject().InstagramViewModel.InstagramModel.Clone();

        public PinterestModel PinterestModel { get; set; } =
            Pinterest.GetSingeltonPinterestObject().PinterestViewModel.PinterestModel.Clone();

        public TumblrModel TumblrModel { get; set; } = Tumblr.GetSingeltonTumblr().TumblrViewModel.TumblrModel.Clone();

        public TwitterModel TwitterModel { get; set; } =
            Twitter.GetSingletonTwitterObject().TwitterViewModel.TwitterModel.Clone();

        public RedditModel RedditModel { get; set; } =
            Reddit.GetSingeltonRedditObject().RedditViewModel.RedditModel.Clone();
    }
}