using DominatorHouseCore.Models;
using PinDominatorCore.Request;
using System;
using System.Windows;

namespace PinDominatorCore.PDModel
{
    public class JsonFunct
    {
        private static readonly object Lock = new object();
        private static volatile JsonFunct Instance;
        public static JsonFunct GetInstance
        {
            get
            {
                if (Instance == null)
                {
                    lock (Lock)
                    {
                        if (Instance == null)
                            Instance = new JsonFunct();
                    }
                }
                return Instance;
            }
        }
        public PdJsonElement GetPostDataFromJsonLogin(DominatorAccountModel accountModel)
        {
            var objPdJsonElement = new PdJsonElement
            {
                SourceUrl = "/",
                Data = new PdJsonElement
                {
                    Options = new PdJsonElement
                    {
                        Email = accountModel.AccountBaseModel.UserName,
                        Password = Uri.EscapeDataString(accountModel.AccountBaseModel.Password),
                        AppTypeFromClient = "5"
                    },

                    Context = new PdJsonElement()
                }
            };
            return objPdJsonElement;
        }

        public PdJsonElement GetPostDataFromJsonLoginForCaptcha(DominatorAccountModel accountModel, string token)
        {
            var objjsonelement = new PdJsonElement
            {
                SourceUrl = "/",
                Data = new PdJsonElement
                {
                    Options = new PdJsonElement
                    {
                        Email = accountModel.AccountBaseModel.UserName,
                        Password = Uri.EscapeDataString(accountModel.AccountBaseModel.Password),
                        AppTypeFromClient = "5",
                        RecaptchaToken = token
                    },

                    Context = new PdJsonElement()
                }
            };
            return objjsonelement;
        }

        public PdJsonElement GetPostDataFromFollowUser(string username, string userId)
        {
            var objPdJsonElement = new PdJsonElement
            {
                SourceUrl = "/" + username + "/_following/",
                Data = new PdJsonElement
                {
                    Options = new PdJsonElement
                    {
                        UserId = userId
                    },
                    Context = new PdJsonElement()
                }
            };
            return objPdJsonElement;
        }

        public PdJsonElement GetPostDataFromFollowUnfollowBoard(string boardUrl, string boardId)
        {
            var objPdJsonElement = new PdJsonElement
            {
                SourceUrl = "/" + boardUrl + "/",
                Data = new PdJsonElement
                {
                    Options = new PdJsonElement
                    {
                        BoardId = boardId
                    },
                    Context = new PdJsonElement()
                }
            };
            return objPdJsonElement;
        }

        public PdJsonElement GetPostDataFromUnfollowUser(string username, string userId)
        {
            var objPdJsonElement = new PdJsonElement
            {
                SourceUrl = "/" + username + "/following/",
                Data = new PdJsonElement
                {
                    Options = new PdJsonElement
                    {
                        UserId = userId
                    },
                    Context = new PdJsonElement()
                }
            };
            return objPdJsonElement;
        }

        public PdJsonElement GetPostDataFromComment(string pinId, string objectId, string message)
        {
            var objPdJsonElement = new PdJsonElement
            {
                SourceUrl = "/pin/" + pinId + "/",
                Data = new PdJsonElement
                {
                    Options = new PdJsonElement
                    {
                        ObjectId = objectId,
                        PinId = pinId,
                        Text = message
                    },
                    Context = new PdJsonElement()
                }
            };
            return objPdJsonElement;
        }

        public PdJsonElement GetPostDataFromCustomRePin(string pinId, string boardId, string trackingParam,
           string description, string link, string username)
        {
            var objPdJsonElement = new PdJsonElement
            {
                SourceUrl = "/"+"pin"+ "/"+ pinId + "/",
                Data = new PdJsonElement
                {
                    Options = new PdJsonElement
                    {
                        BoardId = boardId,
                        TrackingParam = trackingParam,
                        Description = Uri.EscapeDataString(description).Replace("%5C", "%5C%5C")
                            .Replace("%22", "%5C%22"),
                        Link = Uri.EscapeDataString(link),
                        IsBuyablePin = false,
                        IsRemovable = false,
                        PinUnderscoreId = pinId,
                        Title = "",                        
                    },
                    Context = new PdJsonElement()
                }
            };
            return objPdJsonElement;
        }

        public PdJsonElement GetPostDataFromRePin(string pinId, string boardId, string trackingParam,
            string description, string link,string username)
        {
            var objPdJsonElement = new PdJsonElement
            {
                SourceUrl =username,
                Data = new PdJsonElement
                {
                    Options = new PdJsonElement
                    {                       
                        BoardId =boardId,
                        TrackingParam = trackingParam,
                        Description = Uri.EscapeDataString(description).Replace("%5C", "%5C%5C")
                            .Replace("%22", "%5C%22"),
                        Link = Uri.EscapeDataString(link),
                        IsBuyablePin =false,
                        IsRemovable =false,
                        PinUnderscoreId = pinId,
                        Title = "",
                        CommerceData= "{\"is_rich_product_pin\":true}"
                    },
                    Context = new PdJsonElement()
                }
            };
            return objPdJsonElement;
        }

        public PdJsonElement GetPostDataFromCreateBoard(string profileId, string boardName, string description,
            string category,bool KeepBoardSecret=false)
        {
            var objPdJsonElement = new PdJsonElement
            {
                SourceUrl = "/" + profileId + "/",
                Data = new PdJsonElement
                {
                    Options = new PdJsonElement
                    {
                        Name = boardName,
                        Description = description,
                        Category = category,
                        Privacy =KeepBoardSecret ? "secret" : "public",
                        CollabBoardEmail = true,
                        CollaboratorInvitesEnabled = true
                    },
                    Context = new PdJsonElement()
                }
            };
            return objPdJsonElement;
        }

        public PdJsonElement GetPostDataFromCreateSectionBoard(string pinid, string boardid, string sectionname)
        {
            var objPdJsonElement = new PdJsonElement
            {
                SourceUrl = "/" + pinid + "/",
                Data = new PdJsonElement
                {
                    Options = new PdJsonElement
                    {
                        BoardId = boardid,
                        initial_pin = new Array[0],
                        Name = sectionname,
                        name_source = "0",
                        nofetchcontext = false,
                    },
                    Context = new PdJsonElement()
                }
            };
            return objPdJsonElement;
        }

        public PdJsonElement GetPostDataFromEditPin(string profileId, string board, string boardId, string boardSectionId,
            string title, string description, string pinId, string webUrl)
        {
            var objPdJsonElement = new PdJsonElement
            {
                SourceUrl = $"/pin/{pinId}/",
                Data = new PdJsonElement
                {
                    Options = new PdJsonElement
                    {
                        BoardId = boardId,
                        BoardSectionId = boardSectionId,
                        Description = description,
                        DisableComments = false,
                        DisableDidIt = false,
                        Id = pinId,
                        Link = webUrl,
                        Title = title
                    },
                    Context = new PdJsonElement()
                }
            };
            return objPdJsonElement;
        }

        public PdJsonElement GetPostDataFromLikeSomeonesComment(string pinId, string commentId)
        {
            var objPdJsonElement = new PdJsonElement
            {
                SourceUrl = "/pin/" + pinId + "/",
                Data = new PdJsonElement
                {
                    Options = new PdJsonElement
                    {
                        AggregatedCommentId = commentId
                    },
                    Context = new PdJsonElement()
                }
            };
            return objPdJsonElement;
        }

        public PdJsonElement GetPostDataFromDeletePins(string pinId, string userName)
        {
            var objPdJsonElement = new PdJsonElement
            {
                SourceUrl = "/" + "pin" + "/"+pinId,
                Data = new PdJsonElement
                {
                    Options = new PdJsonElement
                    {
                        Id = pinId
                    },
                    Context = new PdJsonElement()
                }
            };
            return objPdJsonElement;
        }

        public PdJsonElement GetDataFromUsersFollowersAndFollowings(string userName, string[] bookmark = null)
        {
            var objPdJsonElement = new PdJsonElement
            {
                Options = new PdJsonElement
                {
                    Bookmarks = bookmark,
                    HideFindFriendsRep = true,
                    Username = userName
                },
                Context = new PdJsonElement()
            };
            return objPdJsonElement;
        }

        public PdJsonElement GetUsersDataFromKetwords(string keyword, string[] bookmark = null)
        {
            var objPdJsonElement = new PdJsonElement
            {
                Options = new PdJsonElement
                {
                    Bookmarks = bookmark,
                    AutoCorrectionDisabled = false,
                    Corpus = null,
                    CustomizedRerankType = null,
                    Filters = null,
                    Query = keyword,
                    QueryPinSigs = null,
                    ReduxNormalizeFeed = false,
                    Rs = "typed",
                    Scope = "users",
                    Article = null
                },
                Context = new PdJsonElement()
            };
            return objPdJsonElement;
        }

        public PdJsonElement GetDataFromBoardFollowers(string boardId, string[] bookmark = null)
        {
            var objPdJsonElement = new PdJsonElement
            {
                Options = new PdJsonElement
                {
                    BoardId = boardId,
                    Bookmarks = bookmark,
                    PageSize = 50
                },
                Context = new PdJsonElement()
            };
            return objPdJsonElement;
        }

        public PdJsonElement GetBoardsDataFromKetwords(string keyword, string[] bookmark = null)
        {
            var objPdJsonElement = new PdJsonElement
            {
                Options = new PdJsonElement
                {
                    Bookmarks = bookmark,
                    AutoCorrectionDisabled = false,
                    Query = keyword,
                    ReduxNormalizeFeed = false,
                    Rs = "typed",
                    Scope = "boards"
                },
                Context = new PdJsonElement()
            };
            return objPdJsonElement;
        }

        public PdJsonElement GetBoardsDataFromUser(string user)
        {
            var objPdJsonElement = new PdJsonElement
            {
                Options = new PdJsonElement
                {
                    Filter = "public",
                    Limit = 500,
                    Sort = "last_pinned_to",
                    FieldSetKey = "profile_grid_item",
                    SkipBoardCreateRep = true,
                    Username = user
                },
                Context = new PdJsonElement()
            };
            return objPdJsonElement;
        }

        public PdJsonElement GetUserBoardData(string user, int count, string[] bookmark = null)
        {
            var pdJsonElement = new PdJsonElement
            {
                Options = new PdJsonElement
                {
                    Bookmarks = bookmark,
                    IsPrefetch = false,
                    PrivacyFilter = "all",
                    Sort = "last_pinned_to",
                    FieldSetKey = "profile_grid_item",
                    Username = user,
                    PageSize = count,
                    GroupBy = "visibility",
                    IncludeArchived = true,
                    ReduxNormalizeFeed = true
                },
                Context = new PdJsonElement()
            };
            return pdJsonElement;
        }

        public PdJsonElement GetPinsDataFromKetwords(string keyword, string[] bookmark = null)
        {
            var objPdJsonElement = new PdJsonElement
            {
                Options = new PdJsonElement
                {
                    Bookmarks = bookmark,
                    IsPrefetch = false,
                    Article = null,
                    AutoCorrectionDisabled = false,
                    Corpus = null,
                    CustomizedRerankType = null,
                    Filter = null,
                    PageSize = null,
                    Query = keyword,
                    QueryPinSigs = null,
                    ReduxNormalizeFeed = true,
                    Rs = "typed",
                    Scope = "pins",
                    SourceId = null
                },
                Context = new PdJsonElement()
            };
            return objPdJsonElement;
        }

        public PdJsonElement GetPinsDataFromBoardUrl(string boardId, string boardUrl, string[] bookmark = null)
        {
            var objPdJsonElement = new PdJsonElement
            {
                Options = new PdJsonElement
                {
                    Bookmarks = bookmark,
                    BoardId = boardId,
                    BoardUrl = boardUrl,
                    FilterSectionPins = true,
                    Layout = "default",
                    Sort = "default",
                    PageSize = 25,
                    ReduxNormalizeFeed = true,
                    CurrentFilter = -1,
                    FieldSetKey= "react_grid_pin"
                },
                Context = new PdJsonElement()
            };
            return objPdJsonElement;
        }

        public PdJsonElement GetUsersDataFromWhoTriedPin(string[] bookmarks, string pinDataId)
        {
            var objPdJsonElement = new PdJsonElement
            {
                Options = new PdJsonElement
                {
                    Bookmarks = bookmarks,
                    AggregatedPinDataId = pinDataId,
                    PageSize = 6,
                    ShowDidItFeed = true,
                    FieldSetKey = "did_it"
                },
                Context = new PdJsonElement()
            };
            return objPdJsonElement;
        }

        public PdJsonElement GetPinsDataForSpecificUser(string[] bookmarks, string userName)
        {
            var objPdJsonElement = new PdJsonElement
            {
                Options = new PdJsonElement
                {
                    Bookmarks = bookmarks,
                    IsOwnProfilePins = false,
                    Username = userName,
                    FieldSetKey = "grid_item"
                },
                Context = new PdJsonElement()
            };
            return objPdJsonElement;
        }

        public PdJsonElement GetPostDataFromPost(string boardId, string userName, string title, string description,
            string link, string imageUrl)
        {
            if (link == null)
                link = string.Empty;
            var objPdJsonElement = new PdJsonElement
            {
                SourceUrl= "/pin-builder/",
                Data = new PdJsonElement
                {
                    Options = new PdJsonElement
                    {
                        UploadMetric = new PdJsonElement
                        {
                            Source = "pinner_upload_standalone"
                        },
                        BoardId = boardId,
                        Description = description,
                        Link = link,
                        ImageUrl = imageUrl,
                        IsBuyablePin = false,
                        Method = "uploaded",
                        Title = title,
                        ShareFacebook = false,
                        ShareTwitter = false
                    },
                    Context = new PdJsonElement()
                }
            };
            return objPdJsonElement;
        }

        public PdJsonElement GetPostDataFromAccepBoard(PinterestBoard pinterestBoard)
        {
            var objPdJsonElement = new PdJsonElement
            {
                SourceUrl = pinterestBoard.BoardUrl,
                Data = new PdJsonElement
                {
                    Options = new PdJsonElement
                    {
                        BoardId = pinterestBoard.Id,
                        InvitedUserId = pinterestBoard.PinterestUserRecipient.UserId
                    },
                    Context = new PdJsonElement()
                }
            };
            return objPdJsonElement;
        }

        public PdJsonElement GetPostDataFromSendBoardInvitation(string boardId, string username, string email,string Boardurl,string Collaborater_id)
        {
            var objPdJsonElement = new PdJsonElement
            {
                SourceUrl = Boardurl.Replace("https://www.pinterest.com", ""),
                Data = new PdJsonElement
                {
                    Options = new PdJsonElement
                    {
                        BoardId = boardId,
                        invitedUserId = new[] { Collaborater_id }
                    },
                    Context = new PdJsonElement()
                }
            };
            return objPdJsonElement;
        }

        public PdJsonElement GetPostDataFromAccepMessage(PinterestUser pinterestUser)
        {
            var objPdJsonElement = new PdJsonElement
            {
                SourceUrl = $"/{pinterestUser.PinterestUserRecipient.Username}/",
                Data = new PdJsonElement
                {
                    Options = new PdJsonElement
                    {
                        ContactRequest = new PdJsonElement
                        {
                            Sender = new PdJsonElement
                            {
                                Username = pinterestUser.PinterestUserSender.Username,
                                FirstName = pinterestUser.PinterestUserSender.FirstName,
                                LastName = pinterestUser.PinterestUserSender.LastName,
                                Gender = pinterestUser.PinterestUserSender.Gender.ToString(),
                                ImageMediumUrl = pinterestUser.PinterestUserSender.ImageMediumUrl,
                                ImageXlargeUrl = pinterestUser.PinterestUserSender.ImageXlargeUrl,
                                FullName = pinterestUser.PinterestUserSender.FullName,
                                ImageSmallUrl = pinterestUser.PinterestUserSender.ImageSmallUrl,
                                Type = pinterestUser.PinterestUserSender.Type,
                                Id = pinterestUser.PinterestUserSender.UserId,
                                ImageLargeUrl = pinterestUser.PinterestUserSender.ImageLargeUrl
                            },
                            Read = pinterestUser.Read,
                            Recipient = new PdJsonElement
                            {
                                Username = pinterestUser.PinterestUserRecipient.Username,
                                FirstName = pinterestUser.PinterestUserRecipient.FirstName,
                                LastName = pinterestUser.PinterestUserRecipient.LastName,
                                Gender = pinterestUser.PinterestUserRecipient.Gender.ToString(),
                                ImageMediumUrl = pinterestUser.PinterestUserRecipient.ImageMediumUrl,
                                ImageXlargeUrl = pinterestUser.PinterestUserRecipient.ImageXlargeUrl,
                                FullName = pinterestUser.PinterestUserRecipient.FullName,
                                ImageSmallUrl = pinterestUser.PinterestUserRecipient.ImageSmallUrl,
                                Type = pinterestUser.PinterestUserRecipient.Type,
                                Id = pinterestUser.PinterestUserRecipient.UserId,
                                ImageLargeUrl = pinterestUser.PinterestUserRecipient.ImageLargeUrl
                            },
                            CreatedAt = pinterestUser.CreatedAt,
                            Conversation = pinterestUser.Conversation,
                            Board = null,
                            Type = pinterestUser.Type,
                            Id = pinterestUser.ContactRequestId,
                            IsInboxRedesign = true
                        },
                    },
                    Context = new PdJsonElement()
                }
            };
            return objPdJsonElement;
        }

        public PdJsonElement GetPostDataFromLoginHandshake(string token)
        {
            var objjsonelement = new PdJsonElement
            {
                SourceUrl = "/",
                Data = new PdJsonElement
                {
                    Options = new PdJsonElement
                    {
                        Token = token,
                        IsRegistration = false
                    },

                    Context = new PdJsonElement()
                }
            };
            return objjsonelement;
        }

        public PdJsonElement GetPostDataForLogout(DominatorAccountModel accountModel)
        {
            var pdJsonElement = new PdJsonElement
            {
                SourceUrl = "/",
                Data = new PdJsonElement
                {
                    Options = new PdJsonElement
                    {
                        DisableAuth = "true",
                        SavePassword = "false"
                    },

                    Context = new PdJsonElement()
                }
            };

            return pdJsonElement;
        }

        public PdJsonElement GetUserConnectedWithMessage(string[] bookmarks)
        {
            var objPdJsonElement = new PdJsonElement
            {
                Options = new PdJsonElement
                {
                    Bookmarks = bookmarks,
                    IsPrefetch = false,
                    FieldSetKey = "default"
                },
                Context = new PdJsonElement()
            };
            return objPdJsonElement;
        }

        public PdJsonElement GetBusinessIdUrl(string userId)
        {
            var pdJsonElement = new PdJsonElement
            {
                Options = new PdJsonElement
                {
                    IsPrefetch = false,
                    FieldSetKey = "linked_partner",
                    UserId = userId
                },
                Context = new PdJsonElement()
            };

            return pdJsonElement;
        }

        public PdJsonElement GetPostDataForSwitchToBusiness(string username, string businessId)
        {

            var pdJsonElement = new PdJsonElement
            {
                SourceUrl = $"/{username}/",
                Data = new PdJsonElement
                {
                    Options = new PdJsonElement
                    {
                        BusinessId = businessId,
                        GetUser = true,
                        AppTypeFromClient = "5",
                        VisitedPagesBeforeLogin = null
                    },
                    Context = new PdJsonElement()
                }
            };
            return pdJsonElement;
        }

        public PdJsonElement GetPostDataForSwitchToPrivate()
        {
            var pdJsonElement = new PdJsonElement
            {
                SourceUrl = "/",
                Data = new PdJsonElement
                {
                    Options = new PdJsonElement
                    {
                        DisableAuth = "true"
                    },
                    Context = new PdJsonElement()
                }
            };

            return pdJsonElement;
        }

        public PdJsonElement GetPostDataForUserRegTrackActRes(string Domain)
        {
            var pdJsonElement = new PdJsonElement
            {
                Options =new PdJsonElement
                    {
                        Action = new[]
                        {
                            new PdJsonElement
                            {
                                Name="mweb_service_worker.unregister_attempt",
                                AuxData=new PdJsonElement
                                {
                                    TagUser=new PdJsonElement{}
                                }
                            },new PdJsonElement
                            {
                                Name="pinner_conversion.autologin.google.start",
                                AuxData=new PdJsonElement
                                {
                                    TagUser=new PdJsonElement{}
                                }
                            },new PdJsonElement
                            {
                                Name="pinner_conversion.autologin.google.load_script_start",
                                AuxData=new PdJsonElement
                                {
                                    TagUser=new PdJsonElement{}
                                }
                            },new PdJsonElement
                            {
                                Name="mweb.unauth.window_size",
                                AuxData=new PdJsonElement
                                {
                                    Width=SystemParameters.PrimaryScreenWidth.ToString(),
                                    Height=SystemParameters.PrimaryScreenHeight.ToString()
                                }
                            },new PdJsonElement
                            {
                                Name="mweb.connection_status_initial",
                                AuxData=new PdJsonElement
                                {
                                    Downlink="6.25",
                                    EffectiveType="4g",
                                    Rtt="200",
                                    SaveData=false,
                                    IsAuth=false
                                }
                            },new PdJsonElement
                            {
                                Name="mweb.removeAccountDataFromLocalStorage.start",
                                AuxData=new PdJsonElement
                                {
                                    TagUser=new PdJsonElement{}
                                }
                            },new PdJsonElement
                            {
                                Name="mweb.removeAccountDataFromLocalStorage.early_return",
                                AuxData=new PdJsonElement { TagUser=new PdJsonElement{}}
                            },new PdJsonElement
                            {
                                Name="pinner_conversion.autologin.google.load_script_success",
                                AuxData=new PdJsonElement { TagUser=new PdJsonElement{}}
                            },new PdJsonElement
                            {
                                Name="pinner_conversion.autologin.google.initialize_library",
                                AuxData=new PdJsonElement { TagUser=new PdJsonElement{}}
                            },new PdJsonElement
                            {
                                Name="mweb.unauth_seo.referrer",
                                AuxData=new PdJsonElement { TagUser=new PdJsonElement{Referrer=null,DocReferer=Domain }}
                            },new PdJsonElement
                            {
                                Name="unauth_web.unauth_page_wrapper.unknown.mounted.is_mobile_false",
                                AuxData=new PdJsonElement { TagUser=new PdJsonElement{}}
                            },new PdJsonElement
                            {
                                Name="web.unauth.locale_data",
                                AuxData=new PdJsonElement {TimeZone=TimeZoneInfo.Local.DisplayName}
                            },new PdJsonElement
                            {
                                Name="mweb_previously_logged_out.true",
                                AuxData=new PdJsonElement { TagUser=new PdJsonElement{}}
                            },new PdJsonElement
                            {
                                Name="pcons.non_google_autologin_disabled_cookie.-1",
                                AuxData=new PdJsonElement { TagUser=new PdJsonElement{}}
                            },new PdJsonElement
                            {
                                Name="pinner_conversion.autologin.facebook.start",
                                AuxData=new PdJsonElement { TagUser=new PdJsonElement{}}
                            },new PdJsonElement
                            {
                                Name="pcons.non_google_autologin_disabled_cookie.-1",
                                AuxData=new PdJsonElement { TagUser=new PdJsonElement{}}
                            },new PdJsonElement
                            {
                                Name="track_register_action.web.invalid_action.with_percent",
                                AuxData=new PdJsonElement { TagUser=new PdJsonElement{},InvalidAction="facebook.auto_login.catch_all.failure.%5Bobject%20Object%5D"}
                            },new PdJsonElement
                            {
                                Name="pcons.google_autologin_disabled_cookie.-1",
                                AuxData=new PdJsonElement { TagUser=new PdJsonElement{}}
                            },new PdJsonElement
                            {
                                Name="mweb_unauth.mobile_modal.autologin.did_mount.is_open.false",
                                AuxData=new PdJsonElement { TagUser=new PdJsonElement{}}
                            },new PdJsonElement
                            {
                                Name="pcons.google_autologin_disabled_cookie.-1",
                                AuxData=new PdJsonElement { TagUser=new PdJsonElement{}}
                            },new PdJsonElement
                            {
                                Name="pcons.google_autologin_disabled_cookie.-1",
                                AuxData=new PdJsonElement { TagUser=new PdJsonElement{}}
                            },new PdJsonElement
                            {
                                Name="pcons.google_autologin_disabled_cookie.-1",
                                AuxData=new PdJsonElement { TagUser=new PdJsonElement{}}
                            },new PdJsonElement
                            {
                                Name="mweb.unauth.personalized_login.fetch_user_info.info.no_user_info.logged_out_cookie_false",
                                AuxData=new PdJsonElement { TagUser=new PdJsonElement{}}
                            },new PdJsonElement
                            {
                                Name="web.unauth.modal_title.view",
                                AuxData=new PdJsonElement { TagUser=new PdJsonElement{}}
                            },new PdJsonElement
                            {
                                Name="unauth_web_modal.home_page.tier1.signup.shown",
                                AuxData=new PdJsonElement { TagUser=new PdJsonElement{}}
                            },new PdJsonElement
                            {
                                Name="dweb.signup_age_step",
                                AuxData=new PdJsonElement { TagUser=new PdJsonElement{Type="email",Format="birthday"}}
                            },new PdJsonElement
                            {
                                Name="unauth.authentication_modal.shown.SMALL_TOGGLE.signup",
                                AuxData=new PdJsonElement { TagUser=new PdJsonElement{}}
                            },new PdJsonElement
                            {
                                Name="logged_out_product.interaction.home.view",
                                AuxData=new PdJsonElement { TagUser=new PdJsonElement{Item="homepage-section",WithIn="homepage-top-section"}}
                            },new PdJsonElement
                            {
                                Name="pcons.google_autologin_disabled_cookie.-1",
                                AuxData=new PdJsonElement { TagUser=new PdJsonElement{}}
                            },new PdJsonElement
                            {
                                Name="pcons.google_autologin_disabled_cookie.-1",
                                AuxData=new PdJsonElement { TagUser=new PdJsonElement{}}
                            },new PdJsonElement
                            {
                                Name="unauth.login_button.click",
                                AuxData=new PdJsonElement { TagUser=new PdJsonElement{}}
                            },
                            new PdJsonElement
                            {
                                Name="lex.mweb_press_header_login",
                                AuxData=new PdJsonElement { TagUser=new PdJsonElement{}}
                            },new PdJsonElement
                            {
                                Name="logged_out_product.interaction.home.click",
                                AuxData=new PdJsonElement { TagUser=new PdJsonElement{Item="login-button",WithIn="unauth-header"}}
                            },new PdJsonElement
                            {
                                Name="unauth_web_container.home_page.tier1.login.shown",
                                AuxData=new PdJsonElement { TagUser=new PdJsonElement{}}
                            },new PdJsonElement
                            {
                                Name="logged_out_product.interaction.home.trigger",
                                AuxData=new PdJsonElement { TagUser=new PdJsonElement{Item="login-modal",WithIn="none"}}
                            },new PdJsonElement
                            {
                                Name="pcons.google_autologin_disabled_cookie.-1",
                                AuxData=new PdJsonElement { TagUser=new PdJsonElement{}}
                            },new PdJsonElement
                            {
                                Name="web.unauth.modal_title.view",
                                AuxData=new PdJsonElement { TagUser=new PdJsonElement{}}
                            },new PdJsonElement
                            {
                                Name="unauth_web_modal.home_page.tier1.login.shown",
                                AuxData=new PdJsonElement { TagUser=new PdJsonElement{}}
                            },new PdJsonElement
                            {
                                Name="unauth.authentication_modal.shown.NOT_NOW_BUTTON.login",
                                AuxData=new PdJsonElement { TagUser=new PdJsonElement{}}
                            },new PdJsonElement
                            {
                                Name="pinner_conversion.login.email.start",
                                AuxData=new PdJsonElement { TagUser=new PdJsonElement{}}
                            },new PdJsonElement
                            {
                                Name="lex.press_login_continue",
                                AuxData=new PdJsonElement { TagUser=new PdJsonElement{}}
                            },new PdJsonElement
                            {
                                Name="unauth.login.button.clicked",
                                AuxData=new PdJsonElement { TagUser=new PdJsonElement{}}
                            },new PdJsonElement
                            {
                                Name="pinner_conversion",
                                AuxData=new PdJsonElement { TagUser=new PdJsonElement{action="login",Event="attempt",Trigger="other",Type="email",Referrer=Domain }}
                            },new PdJsonElement
                            {
                                Name="pinner_conversion.login.email.login_api_call_start",
                                AuxData=new PdJsonElement { TagUser=new PdJsonElement{}}
                            }
                        }
                    },
                Context = new PdJsonElement() { }
            };
            return pdJsonElement;
        }

        public PdJsonElement GetPostDataForTryImageSig(string pinId, string imageUrl)
        {
            var pdJsonElement = new PdJsonElement
            {
                SourceUrl = $"/pin/{pinId}/",
                Data = new PdJsonElement
                {
                    Options = new PdJsonElement
                    {
                        ImageUrl = imageUrl
                    },
                    Context = new PdJsonElement()
                }
            };

            return pdJsonElement;
        }

        public PdJsonElement GetPostDataForTryPin(string pinId, string note, string imageSign)
        {
            var pdJsonElement = new PdJsonElement
            {
                SourceUrl = $"/pin/{pinId}/",
                Data = new PdJsonElement
                {
                    Options = new PdJsonElement
                    {
                        Details = note,
                        ImageSignatures = $"[{imageSign}]",
                        PinUnderscoreId = pinId,
                        PublishFacebook = false,
                        Context = new PdJsonElement()
                    }
                }
            };

            return pdJsonElement;
        }

        public PdJsonElement GetPostDataForVideoParameters()
        {
            var pdJsonElement = new PdJsonElement
            {
                SourceUrl = "/pin-builder/",
                Data = new PdJsonElement
                {
                    Options = new PdJsonElement
                    {
                        Type = "video",
                        Context = new PdJsonElement()
                    }
                }
            };

            return pdJsonElement;
        }

        public PdJsonElement GetPostForPublishVideo(string boardId, string description, string link, string title, string imageUrl,
            string mediaUploadId)
        {
            var pdJsonElement = new PdJsonElement
            {
                SourceUrl = "/pin-builder/",
                Data = new PdJsonElement
                {
                    Options = new PdJsonElement
                    {
                        BoardId = boardId,
                        FieldSetKey = "create_success",
                        SkipPinCreateLog = true,
                        Description = description,
                        Link = link,
                        Title = title,
                        ImageUrl = imageUrl,
                        MediaUploadId = mediaUploadId,
                        UserMentionTag = new Array[0]
                    },
                    Context = new PdJsonElement()
                }
            };

            return pdJsonElement;
        }

        public PdJsonElement CheckExistingEmailUrl(CreateAccountInfo createAccountInfo)
        {
            var pdJsonElement = new PdJsonElement
            {
                SourceUrl = "/",
                Data = new PdJsonElement
                {
                    Options = new PdJsonElement
                    {
                        Email = createAccountInfo.Email
                    },
                    Context = new PdJsonElement()
                }
            };

            return pdJsonElement;
        }

        public PdJsonElement GetPostDataFromJsonCreateAccount(CreateAccountInfo accountInfo, string countryid, string username)
        {
            var objPdJsonElement = new PdJsonElement
            {
                SourceUrl = "/",
                Data = new PdJsonElement
                {
                    Options = new PdJsonElement
                    {
                        Container = "home_page",
                        Email1 = Uri.EscapeDataString(accountInfo.Email),
                        Password = Uri.EscapeDataString(accountInfo.Password),
                        Age = accountInfo.Age,
                        Country = countryid,
                        SignupSource = "homePage",
                        FirstName = Uri.EscapeDataString(username),
                        LastName = "",
                        HybridTier = "open",
                        Page = "home",
                        UserBehaviorData = new PdJsonElement()
                    },

                    Context = new PdJsonElement()
                }
            };
            return objPdJsonElement;
        }

        public PdJsonElement GetPostDataFromJsonForNextButton(string state, string value)
        {
            var objPdJsonElement = new PdJsonElement
            {
                SourceUrl = "/",
                Data = new PdJsonElement
                {
                    Options = new PdJsonElement
                    {
                        State = state,
                        Value = value
                    },

                    Context = new PdJsonElement()
                }
            };
            return objPdJsonElement;
        }

        public PdJsonElement GetPostDataFromJsonGenderSelection(CreateAccountInfo accountInfo)
        {
            var objPdJsonElement = new PdJsonElement
            {
                SourceUrl = "/",
                Data = new PdJsonElement
                {
                    Options = new PdJsonElement
                    {
                        Gender = accountInfo.Gender.ToLower()
                    },

                    Context = new PdJsonElement()
                }
            };
            return objPdJsonElement;
        }
       
        public PdJsonElement GetPostDataFromJsonStateSelection(string browselocale, string country)
        {
            var objPdJsonElement = new PdJsonElement
            {
                SourceUrl = "/",
                Data = new PdJsonElement
                {
                    Options = new PdJsonElement
                    {
                        Country = country,
                        Locale = browselocale
                    },

                    Context = new PdJsonElement()
                }
            };
            return objPdJsonElement;
        }

        public PdJsonElement GetDataFromJsonFiveChoiceSelection(CreateAccountInfo accountInfo, string interestedid)
        {
            var objPdJsonElement = new PdJsonElement
            {
                SourceUrl = "/",
                Data = new PdJsonElement
                {
                    Options = new PdJsonElement
                    {
                        Interestid = interestedid,
                        LogData = new PdJsonElement
                        {
                            Pos = "0",
                            Source = $"in_{accountInfo.Gender}_static_topics",
                            Gender = accountInfo.Gender,
                        },
                        Referrer = "nux",
                        UserBehaviorData = new PdJsonElement
                        {
                            SignupInterestsPickerTimeSpent = "3908",
                        }
                    },

                    Context = new PdJsonElement()
                }
            };
            return objPdJsonElement;
        }

        public PdJsonElement GetDataFromJsonFollow(string username)
        {
            var objPdJsonElement = new PdJsonElement
            {                
                    Options = new PdJsonElement
                    {
                        IsPrefetch = false,
                        Username = username,
                        FieldSetKey = "profile",
                        nofetchcontext = false
                    },
                    Context = new PdJsonElement()
            };
            return objPdJsonElement;
        }
    }
}