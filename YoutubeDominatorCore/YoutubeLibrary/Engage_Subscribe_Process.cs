using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Request;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YoutubeDominatorCore.Response;
using YoutubeDominatorCore.YDUtility;
using YoutubeDominatorCore.YoutubeModel;
using YoutubeDominatorCore.YoutubeViewModel;

namespace YoutubeDominatorCore.YoutubeLibrary
{
    class Engage_Subscribe_Process:YdJobProcess
    {
        public GrowSubscribers_Subscribe_Model Engage_SubscribeModel { get; set; }

        public Engage_Subscribe_Process() 
            : base("", "", ActivityType.Subscribe, null) { }

        public Engage_Subscribe_Process(string account, string template, ActivityType activityType, TimingRange currentJobTimeRange) 
            : base(account, template, activityType, currentJobTimeRange)
        {
            this.Engage_SubscribeModel = JsonConvert.DeserializeObject<GrowSubscribers_Subscribe_Model>(TemplatesFileManager.Get().FirstOrDefault(x => x.Id == template).ActivitySettings);
            //this.DominatorAccountModel = base.DominatorAccountModel;
        }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            Youtube_Post YoutubePost = (Youtube_Post)scrapeResult.ResultPost;
            GlobusLogHelper.log.Info("Like post => " + YoutubePost.VideoUrl + " with account => " + DominatorAccountModel.AccountBaseModel.UserName + " module => " + ActivityType.ToString());
            var ytFunct = new YoutubeFunctionality((DominatorAccountModel)DominatorAccountModel);
            DominatorHouseCore.Process.JobProcessResult jobProcessResult = new DominatorHouseCore.Process.JobProcessResult();
            try
            {
                LikeResponseHandler response = ytFunct.LikeVideo(YoutubePost.VideoUrl);
                if (response.Success)
                {
                    GlobusLogHelper.log.Info("Like post => " + YoutubePost.VideoUrl + " with account => " + DominatorAccountModel.AccountBaseModel.UserName + " module => " + ActivityType.ToString());
                    NoOfActionPerformedCurrentJob++;
                    NoOfActionPerformedCurrentHour++;
                    NoOfActionPerformedCurrentDay++;

                    //AddFollowedDataToDataBase(scrapeResult);
                    //this.AccountModel.LstFollowings.Add(InstagramUser);
                    //PostFollowProcess(InstagramUser);
                    jobProcessResult.IsProcessSuceessfull = true;
                }
                else
                {
                    GlobusLogHelper.log.Info("Could not Post => " + YoutubePost.VideoUrl + " with account => " + DominatorAccountModel.AccountBaseModel.UserName + " module => " + ActivityType.ToString());
                    jobProcessResult.IsProcessSuceessfull = false;
                }
                Thread.Sleep(this.ModuleSetting.JobConfiguration.DelayBetweenActivity.GetRandom());
            }
            catch (Exception e)
            {
                GlobusLogHelper.log.Error(e.Message);
            }


            return jobProcessResult;
        }
    }
}
