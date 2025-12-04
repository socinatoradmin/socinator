using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DominatorHouseCore.Interfaces;
using Newtonsoft.Json.Linq;

namespace GramDominatorCore.GDModel
{
    [Localizable(false)]
    public class SearchTagIgResponseHandler : GDLibrary.Response.IGResponseHandler
    {
        public SearchTagIgResponseHandler(IResponseParameter response)
            : base(response)
        {
            if (!Success)
                return;
            if(response.Response.Contains("{\"data\":{\"xdt_api__v1__fbsearch__topsearch_connection\""))
            {
                GetData(response.Response);
                return;
            }
            else
            {
                var obj = handler.ParseJsonToJObject(response?.Response);
                var Hastags = handler.GetJArrayElement(handler.GetJTokenValue(obj, "hashtags"));
                if(Hastags!= null && Hastags.HasValues)
                {
                    foreach(var tag in Hastags)
                    {
                        try
                        {
                            int.TryParse(handler.GetJTokenValue(tag, "hashtag", "media_count"),out int media_count);
                            var tagDetails = new TagDetails(
                                id:handler.GetJTokenValue(tag, "hashtag","id"),
                                count: media_count,
                                name:handler.GetJTokenValue(tag, "hashtag","name")
                                );
                            if (!Items.Any(x => x.Id == tagDetails.Id))
                                Items.Add(tagDetails);
                        }
                        catch { }
                    }
                }
            }
        }

        private void GetData(string response)
        {
            var jsonHandler = new DominatorHouseCore.Utility.JsonHandler(response);
            var hashtags = jsonHandler.GetJToken("data", "xdt_api__v1__fbsearch__topsearch_connection","hashtags");
            foreach (var hashtag in hashtags)
            {
                try
                {
                    TagDetails tag = new TagDetails(
                                jsonHandler.GetJTokenValue(hashtag,"hashtag", "id"),
                                int.Parse(jsonHandler.GetJTokenValue(hashtag, "hashtag", "media_count")),
                                jsonHandler.GetJTokenValue(hashtag, "hashtag", "name")

                            );
                    Items.Add(tag);
                }
                catch (Exception)
                {

                }
            }
        }
        public bool HasMore { get; set; } = false;
        public string MaxId { get; set; } = string.Empty;
        public List<TagDetails> Items { get; } = new List<TagDetails>();

    }
}
