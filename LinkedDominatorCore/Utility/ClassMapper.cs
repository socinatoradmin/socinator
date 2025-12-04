using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.LdTables.Account;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using LinkedDominatorCore.LDModel;
using LinkedDominatorCore.LDModel.GrowConnection;
using LinkedDominatorCore.LDModel.Messenger;
using LinkedDominatorCore.LDModel.Profilling;
using LinkedDominatorCore.LDModel.Scraper;
using LinkedDominatorCore.LDUtility;
using Newtonsoft.Json;

namespace LinkedDominatorCore.Utility
{
    public interface IClassMapper
    {
        LinkedinUser InteractedUserToLinkedInUserMapper(InteractedUsers interactedUser);

        List<LinkedinUser>
            InteractedUserToLinkedInUserMapper(List<InteractedUsers> listInteractedUser);
        List<Connections> GetConnectionFromInteractedUsers(List<InteractedUsers> interactedUsers);
        Connections LinkedInUserToConnections(LinkedinUser linkedinUser, IMapper imapper = null);

        void MapModelClass<TSourceModel, TDestinationModel>(TSourceModel sourceModel,
            ref TDestinationModel destinationModel, IMapper imapper = null) where TDestinationModel : class, new();

        TDestinationModel MapModelClass<TSourceModel, TDestinationModel>(TSourceModel sourceModel)
            where TDestinationModel : class, new();

        void MapListOfModelClass<TSourceModel, TDestinationModel>(List<TSourceModel> listSourceModel,
            ref List<TDestinationModel> listDestinationModel) where TDestinationModel : class, new();

        void SetModelClass(ref MapperModel modelClass, ActivityType activityType, IJobProcess ldJobProcess);
        IMapper GetIMapper<TSourceModel, TDestinationModel>();
        LinkedinUser MappedConnectionToLinkedInUser(Connections objConnections);
    }

    public class ClassMapper : IClassMapper
    {
        public static ClassMapper Instance { get; } = new ClassMapper();
        public LinkedinUser InteractedUserToLinkedInUserMapper(InteractedUsers interactedUser)
        {
            var user = new LinkedinUser();
            try
            {
                user = new LinkedinUser
                {
                    ProfileId = interactedUser.ProfileId,
                    FullName = interactedUser.UserFullName,
                    ProfileUrl = interactedUser.UserProfileUrl
                };
            }
            catch (Exception)
            {
                //ignored
            }

            return user;
        }

        public Connections LinkedInUserToConnections(LinkedinUser linkedinUser, IMapper imapper = null)
        {
            var connections = new Connections();
            try
            {
                var iMapper = imapper ?? GetIMapper<LinkedinUser, Connections>();
                connections = iMapper.Map<LinkedinUser, Connections>(linkedinUser);
                connections.IsDetailedUserInfoVisible = true;
                connections.ConnectionType = ConnectionType.FirstDegree;
                connections.InteractionTimeStamp = DateTimeUtilities.GetEpochTime();
            }
            catch (Exception)
            {
                //ignored
            }

            return connections;
        }

        public List<LinkedinUser> InteractedUserToLinkedInUserMapper(List<InteractedUsers> listInteractedUser)
        {
            var listLinkedinUsers = new List<LinkedinUser>();
            foreach (var linkedinUser in listInteractedUser)
                listLinkedinUsers.Add(InteractedUserToLinkedInUserMapper(linkedinUser));
            return listLinkedinUsers;
        }

        public void MapModelClass<TSourceModel, TDestinationModel>(TSourceModel sourceModel,
            ref TDestinationModel destinationModel, IMapper imapper = null) where TDestinationModel : class, new()
        {
            try
            {
                var iMapper = imapper ?? GetIMapper<TSourceModel, TDestinationModel>();
                destinationModel = iMapper.Map<TSourceModel, TDestinationModel>(sourceModel);
            }
            catch (Exception)
            {
                //ignored
            }
        }

        public IMapper GetIMapper<TSourceModel, TDestinationModel>()
        {
            var mapperConfig = new MapperConfiguration(cfg => { cfg.CreateMap<TSourceModel, TDestinationModel>(); });

            return mapperConfig.CreateMapper();
        }

        //for abstract destinationModel since we cannot ref it
        public TDestinationModel MapModelClass<TSourceModel, TDestinationModel>(TSourceModel sourceModel)
            where TDestinationModel : class, new()
        {
            TDestinationModel destinationModel = null;
            try
            {
                var mapperConfig = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<TSourceModel, TDestinationModel>();
                });

                var iMapper = mapperConfig.CreateMapper();
                destinationModel = iMapper.Map<TSourceModel, TDestinationModel>(sourceModel);
            }
            catch (Exception)
            {
                //ignored
            }

            return destinationModel;
        }

        public void MapListOfModelClass<TSourceModel, TDestinationModel>(List<TSourceModel> listSourceModel,
            ref List<TDestinationModel> listDestinationModel) where TDestinationModel : class, new()
        {
            foreach (var sourceModel in listSourceModel)
                try
                {
                    var destinationModel = new TDestinationModel();
                    MapModelClass(sourceModel, ref destinationModel);
                    listDestinationModel.Add(destinationModel);
                }
                catch (Exception)
                {
                    //ignored
                }
        }

        public void SetModelClass(ref MapperModel modelClass, ActivityType activityType, IJobProcess ldJobProcess)
        {
            try
            {
                var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
                switch (activityType)
                {
                    // RemoveConnections
                    case ActivityType.RemoveConnections:
                        var removeConnectionModel = JsonConvert.DeserializeObject<RemoveConnectionModel>(
                            templatesFileManager.Get().FirstOrDefault(x => x.Id == ldJobProcess.TemplateId)
                                ?.ActivitySettings);
                        MapModelClass(removeConnectionModel, ref modelClass);
                        break;

                    // WithdrawConnectionRequest
                    case ActivityType.WithdrawConnectionRequest:
                        var withdrawConnectionRequestModel =
                            JsonConvert.DeserializeObject<WithdrawConnectionRequestModel>(templatesFileManager.Get()
                                .FirstOrDefault(x => x.Id == ldJobProcess.TemplateId)?.ActivitySettings);
                        MapModelClass(withdrawConnectionRequestModel, ref modelClass);
                        break;

                    // ExportConnection
                    case ActivityType.ExportConnection:
                        var exportConnectionModel = JsonConvert.DeserializeObject<ExportConnectionModel>(
                            templatesFileManager.Get().FirstOrDefault(x => x.Id == ldJobProcess.TemplateId)
                                ?.ActivitySettings);
                        MapModelClass(exportConnectionModel, ref modelClass);
                        // preventing next job to schedule
                        ldJobProcess.DominatorAccountModel.IsNeedToSchedule = !exportConnectionModel.IsStopScheduling;
                        break;

                    // BroadcastMessages
                    case ActivityType.BroadcastMessages:
                        var broadcastMessagesModel = JsonConvert.DeserializeObject<BroadcastMessagesModel>(
                            templatesFileManager.Get().FirstOrDefault(x => x.Id == ldJobProcess.TemplateId)
                                ?.ActivitySettings);
                        MapModelClass(broadcastMessagesModel, ref modelClass);
                        break;

                    case ActivityType.ProfileEndorsement:
                        var profileEndorsementModel = JsonConvert.DeserializeObject<ProfileEndorsementModel>(
                            templatesFileManager.Get().FirstOrDefault(x => x.Id == ldJobProcess.TemplateId)
                                ?.ActivitySettings);
                        MapModelClass(profileEndorsementModel, ref modelClass);
                        break;

                    //MessageConversationScraper
                    case ActivityType.AttachmnetsMessageScraper:
                        var MessageConversationScraperModel =
                            JsonConvert.DeserializeObject<MessageConversationScraperModel>(templatesFileManager.Get()
                                .FirstOrDefault(x => x.Id == ldJobProcess.TemplateId)?.ActivitySettings);
                        MapModelClass(MessageConversationScraperModel, ref modelClass);

                        break;

                    // BlockUser
                    case ActivityType.BlockUser:
                        var blockUserModel = JsonConvert.DeserializeObject<BlockUserModel>(templatesFileManager.Get()
                            .FirstOrDefault(x => x.Id == ldJobProcess.TemplateId)?.ActivitySettings);
                        MapModelClass(blockUserModel, ref modelClass);
                        break;
                }

                // in model we given 'ListCustomUser' as 'UrlList' therefore we assigning it
                modelClass.ListCustomUser = modelClass.UrlList;
            }
            catch (Exception exception)
            {
                exception.DebugLog();
            }
        }

        public LinkedinUser MappedConnectionToLinkedInUser(Connections objConnections)
        {
            var publicIdentifier = LdDataHelper.GetInstance.GetPublicInstanceFromProfileUrl(objConnections.ProfileUrl);
            var objLinkedinUser = new LinkedinUser
            {
                PublicIdentifier = publicIdentifier,
                ProfileId = objConnections.ProfileId,
                ProfileUrl = objConnections.ProfileUrl,
                FullName = objConnections.FullName,
                HasAnonymousProfilePicture = objConnections.HasAnonymousProfilePicture,
                ProfilePicUrl = objConnections.ProfilePicUrl,
                ConnectedTimeStamp = objConnections.ConnectedTimeStamp,
                Occupation = objConnections.Occupation
            };
            return objLinkedinUser;
        }

        public List<Connections> GetConnectionFromInteractedUsers(List<InteractedUsers> interactedUsers)
        {
            var connections = new List<Connections>();
            foreach(var user in interactedUsers)
            {
                try
                {
                    var connection = new Connections
                    {
                        FullName = user.UserFullName,
                        ProfileId = user.ProfileId,
                        PublicIdentifier = user.PublicIdentifer,
                        ProfileUrl = user.UserProfileUrl
                    };
                    connection.IsDetailedUserInfoVisible = true;
                    connection.ConnectionType = ConnectionType.FirstDegree;
                    connection.InteractionTimeStamp = DateTimeUtilities.GetEpochTime();
                    connections.Add(connection);
                }
                catch (Exception)
                {
                    //ignored
                }
            }
            return connections;
        }
    }
}