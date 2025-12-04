using System;
using System.Collections.Generic;
using System.Linq;
using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;

namespace QuoraDominatorCore.Response
{
    public class ReadMessageResponseHandler : QuoraResponseHandler
    {
        //should show above 6
        public HashSet<string> Usernameurls;
        List<ChatDetails> chatDetails = new List<ChatDetails>();
        public ReadMessageResponseHandler(IResponseParameter response) : base(response)
        {
            try
            {
                var eachmesg = RespJ["data"]["thread"]["messagesConnection"]["edges"];
                var uFname = RespJ["data"]["thread"]["otherUsers"].Children().FirstOrDefault()["names"].Children().FirstOrDefault()["givenName"].ToString();
                var uLname = RespJ["data"]["thread"]["otherUsers"].Children().FirstOrDefault()["names"].Children().FirstOrDefault()["familyName"].ToString();
                var profileimglURL = RespJ["data"]["thread"]["otherUsers"].Children().FirstOrDefault()["profileImageUrl"].ToString();
                var profilelURL = RespJ["data"]["thread"]["otherUsers"].Children().FirstOrDefault()["profileUrl"].ToString();
                foreach (var eachnode in eachmesg)
                {
                    try
                    {
                        var fname = eachnode["node"]["author"]["names"].Children().FirstOrDefault()["givenName"].ToString();
                        var lname = eachnode["node"]["author"]["names"].Children().FirstOrDefault()["familyName"].ToString();
                        chatDetails.Add(new ChatDetails
                        {
                            
                            Time = eachnode["node"]["time"].ToString(),
                            MessegesId = RespJ["data"]["thread"]["threadId"].ToString(),
                            Messeges = Utilities.GetBetween(eachnode["node"]["content"].ToString(), "{\"text\": \"", "\""),
                            Sender = fname+lname,
                            SenderId= eachnode["node"]["author"]["uid"].ToString(),
                            //MessegeType = eachnode["node"]["__typename"].ToString()
                            
                            
                            //UserProfilePic = profileimglURL,
                            //LastMessage = Utilities.GetBetween(eachnode["node"]["content"].ToString(), "{\"text\": \"", "\""),
                            //UserFullName = uFname + " " + uLname,
                            //MessageDateTime = eachnode["node"]["time"].ToString(),
                            //MessageId = RespJ["data"]["thread"]["threadId"].ToString(),
                            //UserId = eachnode["node"]["author"]["uid"].ToString()
                        });
                    }
                    catch (Exception) { }

                }
                chatDetails.Reverse();
                Usernameurls = new HashSet<string>();
                var temp = HtmlDocument.DocumentNode.SelectNodes("//a[@class='user']").ToArray();
                foreach (var item in temp)
                    try
                    {
                        Usernameurls.Add("https://www.quora.com" + item.Attributes["href"].Value);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                //usernameurl = htmlDocument.DocumentNode.SelectNodes("//a[@class='user']").Last().Attributes["href"].Value;
            }
        }
    }
}