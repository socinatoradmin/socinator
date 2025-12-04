using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using Newtonsoft.Json.Linq;
using PinDominatorCore.PDEnums;
using PinDominatorCore.PDModel;
using PinDominatorCore.Response;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PinDominatorCore.Response
{
    public class AdsScraperResponseHandler : PdResponseHandler
    {
        public List<AdsDataModel> LstPin { get; } = new List<AdsDataModel>();
        public bool HasMoreResults { get; set; }
        public string BookMark { get; set; }

        public AdsScraperResponseHandler(IResponseParameter response, Dictionary<IpLocationDetails, string> location) : base(response)
        {

            if (string.IsNullOrEmpty(response.Response))
            {
                Success = false;
                return;
            }

            var jsonHand = new DominatorHouseCore.Utility.JsonHandler(response.Response);
            JToken jsonToken = null;

            try
            {
                jsonToken = jsonHand.GetJToken("resource_response", "data");

                var bookMark = jsonHand.GetElementValue("resource_response", "bookmark");
                if (string.IsNullOrEmpty(bookMark))
                {
                    bookMark = jsonHand.GetElementValue("resource_response", "bookmark");
                }
                if (string.IsNullOrEmpty(bookMark))
                {
                    bookMark = jsonHand.GetJTokenValue(jsonHand.GetJToken("resources", "data", "BaseSearchResource").First().First(),
                        "nextBookmark");
                }
                BookMark = bookMark;
                HasMoreResults = !bookMark.Contains("end");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            if (jsonToken != null)
                foreach (JToken token in jsonToken)
                {
                    var is_promoted = jsonHand.GetJTokenValue(token, "is_promoted");
                    if (is_promoted.Contains("True"))
                    {

                        var ad_id = jsonHand.GetJTokenValue(token, "id");
                        var ad_title = jsonHand.GetJTokenValue(token, "grid_title");
                        if (string.IsNullOrEmpty(ad_title))
                            ad_title = jsonHand.GetJTokenValue(token, "title");
                        var ad_text = jsonHand.GetJTokenValue(token, "domain");
                        var ad_destination_url = jsonHand.GetJTokenValue(token, "ad_destination_url");

                        var ad_image_url = jsonHand.GetJTokenValue(token, "images", "orig", "url");
                        //var adImageUrl_Base64 = ConvertIntoBase64(ad_image_url).Trim();

                        var ad_description = jsonHand.GetJTokenValue(token, "description");
                        var ad_created_date = jsonHand.GetJTokenValue(token, "created_at");
                        var ad_url = jsonHand.GetJTokenValue(token, "link");

                        //post owner details
                        var post_owner_username = jsonHand.GetJTokenValue(token, "promoter", "username");
                        var post_owner_fullname = jsonHand.GetJTokenValue(token, "promoter", "full_name");

                        var post_owner_imageUrl = jsonHand.GetJTokenValue(token, "promoter", "image_medium_url");
                        //var PostOwner_Imageurl_base64 = ConvertIntoBase64(post_owner_imageUrl);
                        string[] PostOwnerImageUrl_Array = post_owner_imageUrl.Split(',');

                        var post_site_name = jsonHand.GetJTokenValue(token, "rich_summary", "site_name");

                        //pin details
                        var pin_id = jsonHand.GetJTokenValue(token, "pinner", "id");
                        var pin_fullname = jsonHand.GetJTokenValue(token, "pinner", "full_name");
                        var video_id = jsonHand.GetJTokenValue(token, "videos", "id");
                        var video_url = jsonHand.GetJTokenValue(token, "videos", "video_list", "V_HLSV4", "url");


                        string ImageOrVideotype = string.Empty;
                        string[] adImageUrl_array = new string[] { };
                        string[] video_url_array = new string[] { };
                        if (string.IsNullOrEmpty(video_url))
                        {
                            ImageOrVideotype = "Image";
                            adImageUrl_array = ad_image_url.Split(',');
                        }
                        else
                        {
                            ImageOrVideotype = "Video";
                            video_url = video_url.Replace("hls", "720p").Replace("m3u8", "mp4");
                            video_url_array = video_url.Split(',');
                        }


                        var adsdetails = new AdsDataModel()
                        {
                            ad_id = ad_id,
                            ad_title = ad_title,
                            destination_url = ad_destination_url,
                            type = ImageOrVideotype,
                            ad_video = video_url_array,
                            ad_image = adImageUrl_array,
                            ad_text = ad_text,
                            post_owner_image = PostOwnerImageUrl_Array,
                            post_owner = post_owner_username,
                            target_keyword = null,
                            ad_sub_position = null,
                            newsfeed_description = null,
                            city = location[IpLocationDetails.City].ToString(),
                            state = location[IpLocationDetails.State].ToString(),
                            country = location[IpLocationDetails.Country].ToString(),
                            ip_address = location[IpLocationDetails.Ip].ToString()

                        };
                        LstPin.Add(adsdetails);

                    }
                }

        }
       
    }
}
