using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Tumblr.classes;

namespace Tumblr.Page
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : UserControl
    {
        public Login()
        {
            InitializeComponent();
        }
        string email = string.Empty;
        string Password = string.Empty;
        string form_key = string.Empty;
        GlobusHttpHelper obj_Helper = new GlobusHttpHelper();
        private void btn_clicknow(object sender, RoutedEventArgs e)
        {
            email = textBox.Text;
            Password = stextBox.Text;
            string Url = obj_Helper.getHtmlfromUrl(new Uri("http://tumblr.com"));
            string Login_url = "https://www.tumblr.com/login";
            Url = "https://www.tumblr.com/login";
            string Response_ = obj_Helper.getHtmlfromUrl(new Uri(Url));
            form_key = obj_Helper.getBetween(Response_, " id=\"tumblr_form_key\" content=\"", "\">");
            form_key = Uri.EscapeDataString(form_key);
            email= Uri.EscapeDataString(email);
            Password= Uri.EscapeDataString(Password);
            string Post_data = "determine_email="+ email +"&user%5Bemail%5D="+ email + "&user%5Bpassword%5D="+ Password + "&tumblelog%5Bname%5D=&user%5Bage%5D=&context=home_signup&version=STANDARD&follow=&http_referer=https%3A%2F%2Fwww.tumblr.com%2F&form_key="+ form_key + "&seen_suggestion=0&used_suggestion=0&used_auto_suggestion=0&about_tumblr_slide=&random_username_suggestions=%5B%22GrandCollectionPenguin%22%2C%22SwagTyrantPeace%22%2C%22UnabashedlySillyRunaway%22%2C%22OptimisticTastemakerTrash%22%2C%22GenerouslyMagicalPirate%22%5D&action=signup_determine";


            try
            {
                string LoginResp = obj_Helper.postFormData(new Uri(Login_url), Post_data, "https://www.tumblr.com/login");
            }
            catch (Exception ex)
            {
            }
            string reblog = "https://www.tumblr.com/svc/post/update";

            string Postdata_Reblg = "{\"channel_id\":\"basundhara230\",\"post[date]\":\"\",\"post[publish_on]\":\"\",\"post[slug]\":\"\",\"post[tags]\":\"\",\"post[two]\":\"<p>looking fantastic</p>\",\"post[three]\":\"http://imageorca.tumblr.com/post/171188842729/new-free-stock-photo-of-healthy-hand-fruits\",\"editor_type\":\"rich\",\"send_to_twitter\":true,\"custom_tweet\":false,\"send_to_fbog\":true,\"loggingData\":{},\"carousel_display\":false,\"members_only\":false,\"owner_flagged_nsfw\":false,\"content_stats\":{\"elapsed\":47,\"htmlLength\":24,\"textLength\":17,\"textTools\":{\"hr\":0,\"h2\":0,\"more\":0,\"mentions\":0},\"embeds\":{},\"images\":{},\"mentions\":{},\"gifs\":{\"searches\":0,\"added\":0,\"kept\":0},\"gifSearch\":{}},\"carousel_index\":null,\"can_be_liked\":true,\"can_be_reblogged\":true,\"enable_cta\":false,\"cta_text_code\":\"0\",\"enable_redirect_urls\":false,\"redirect_url_primary\":\"\",\"redirect_url_ios\":\"\",\"redirect_url_android\":\"\",\"post[source_url]\":\"http://imageorca.tumblr.com/post/171188842729/new-free-stock-photo-of-healthy-hand-fruits\",\"post[state]\":\"0\",\"post[type]\":\"photo\",\"reblog\":true,\"reblog_key\":\"XV8lJmYD\",\"reblog_post_id\":\"172021567538\",\"context_id\":\"basundhara230\",\"context_page\":\"dashboard\",\"is_rich_text[one]\":\"0\",\"is_rich_text[two]\":\"1\",\"is_rich_text[three]\":\"0\",\"reblog_source\":\"POST_CONTEXT_BLOG_NETWORK\",\"images[o1]\":\"\",\"caption[o1]\":\"\",\"photo_redirect_url_primary[o1]\":\"\",\"photo_redirect_url_ios[o1]\":\"\",\"photo_redirect_url_android[o1]\":\"\",\"post[carousel_display]\":false,\"post[redirect_url_primary]\":\"\",\"post[photoset_layout]\":false,\"post[photoset_order]\":\"o1\"}";

         
            string JSONString = Postdata_Reblg.Replace("null", "\"\"");
             Postdata_Reblg = Uri.EscapeDataString(JSONString);
            try
            {
                string FollowResp = obj_Helper.postFormData(new Uri(reblog), Postdata_Reblg, "https://www.tumblr.com/reblog/172021567538/XV8lJmYD");
            }
            catch (Exception ex)
            {
            }
             string LikePic= "https://www.tumblr.com/svc/like";
            string LikePostdata = "data%5Bid%5D=172021567538&data%5Bkey%5D=XV8lJmYD&data%5Bplacement_id%5D=false&data%5Bis_recommended%5D=&data%5Bmethod%5D=mouse&data%5Bsource%5D=LIKE_SOURCE_IFRAME";
            try
            {
                string FollowResp = obj_Helper.postFormData(new Uri(LikePic), LikePostdata, "");
            }
            catch (Exception ex)
            {
            }
            #region follow and unfollow request

            string FollowUrl = "https://www.tumblr.com/svc/follow";
            string FollowPostdata = "data%5Btumblelog%5D=foodfuck&data%5Bpage_url%5D=https%3A%2F%2Fwww.tumblr.com%2Fsearch%2Fflower&data%5Bsource%5D=FOLLOW_SOURCE_PEEPR_HEADER";
            try
            {
                string FollowResp = obj_Helper.postFormData(new Uri(FollowUrl), FollowPostdata, "https://www.tumblr.com/search/flower");
            }
            catch (Exception ex)
            {
            }
            string UnFollowUrl = "https://www.tumblr.com/svc/unfollow";
            string UnFollowPostdata = "data%5Btumblelog%5D=foodfuck&data%5Bpage_url%5D=https%3A%2F%2Fwww.tumblr.com%2Fsearch%2Fflower&data%5Bsource%5D=UNFOLLOW_SOURCE_PEEPR_HEADER";
            try
            {
                string FollowResp = obj_Helper.postFormData(new Uri(UnFollowUrl), UnFollowPostdata, "https://www.tumblr.com/search/flower");
            }
            catch (Exception ex)
            {
            } 
            #endregion


        }
    }
}
