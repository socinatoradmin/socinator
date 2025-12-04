using DominatorHouseCore.Interfaces;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDResponse.BaseResponse;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace FaceDominatorCore.FDResponse.AccountsResponse
{

    public class AccountMarketplaceDetailsHandler : FdResponseHandler
    {
        public LocationDetails LocationDetails { get; set; } = new LocationDetails();

        public string Currency { get; set; } = string.Empty;

        public List<ProductCategoryDetails> ListCategories = new List<ProductCategoryDetails>();

        public AccountMarketplaceDetailsHandler(IResponseParameter responseParameter)
            : base(responseParameter)
        {
            if (responseParameter.HasError)
                return;

            try
            {
                JObject jObject = JObject.Parse(responseParameter.Response);
                JToken data = jObject["data"]["viewer"]["marketplace_settings"]["categories"]["edges"];
                foreach (JToken dat in data)
                {
                    ProductCategoryDetails category = new ProductCategoryDetails();
                    category.CategoryId = dat["node"]["id"].ToString();
                    category.CategoryName = dat["node"]["name"].ToString();
                    ListCategories.Add(category);
                }

                Currency = jObject["data"]["viewer"]["marketplace_settings"]["current_marketplace"]["primary_currency"].ToString();


                LocationDetails.LocationId =
                    long.Parse(jObject["data"]["viewer"]["marketplace_feed_stories"]["buy_location"]["id"].ToString());
                LocationDetails.Name = jObject["data"]["viewer"]["marketplace_feed_stories"]["buy_location"]["display_name"].ToString();
                LocationDetails.FbLocationName = jObject["data"]["viewer"]["marketplace_feed_stories"]["buy_location"]["marketplace_vanity_id"].ToString();
                LocationDetails.Longitude = jObject["data"]["viewer"]["marketplace_feed_stories"]["buy_location"]["location"]["longitude"].ToString();
                LocationDetails.Latitude = jObject["data"]["viewer"]["marketplace_feed_stories"]["buy_location"]["location"]["latitude"].ToString();

            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}
