using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using PinDominatorCore.PDLibrary.DAL;
using PinDominatorCore.PDModel;
using System;
using System.Collections.Generic;

namespace PinDominatorCore.PDLibrary.Processors.AccountCreator
{
    public class BaseAccountCreatorProcessor : BasePinterestProcessor
    {
        public static readonly object Lock = new object();
        public static Queue<CreateAccountInfo> QueueCreateAccount = new Queue<CreateAccountInfo>();

        public BaseAccountCreatorProcessor(IPdJobProcess jobProcess,
            IDbGlobalService globalService, IDbCampaignService campaignService, IPinFunction objPinFunct) :
            base(jobProcess, globalService, campaignService, objPinFunct)
        {

        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                queryInfo.QueryType = "Create Account";
                var createAccModel = JsonConvert.DeserializeObject<AccountCreatorModel>(TemplateModel.ActivitySettings);
                var accDetails = createAccModel.ObsCreateAccountInfo;
                AddToQueue(accDetails);
                lock (Lock)
                {
                    if (QueueCreateAccount.Count == 0)
                    {
                        DominatorHouseCore.LogHelper.GlobusLogHelper.log.Info(Log.CustomMessage,
                            JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                            "There is no AccountInfo to create", ActivityType);

                        return;
                    }
                }

                lock (Lock)
                {
                    while (QueueCreateAccount.Count > 0)
                    {
                        var lstCreateAccountInfo = RetriveFromQueue();
                        StartFinalProcess(ref jobProcessResult, lstCreateAccountInfo, queryInfo);
                    }
                }
            }
            catch
            {
                // ignored
            }
        }


        public void StartFinalProcess(ref JobProcessResult jobProcessResult, CreateAccountInfo createAccountInfo, QueryInfo queryInfo)
        {
            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            jobProcessResult = JobProcess.FinalProcess(new ScrapeResultNew
            {
                ResultPost = createAccountInfo,
                QueryInfo = queryInfo
            });
        }

        private void AddToQueue(ObservableCollectionBase<CreateAccountInfo> accDetails)
        {
            try
            {
                lock (Lock)
                {
                    if (QueueCreateAccount.Count > 0)
                        return;

                    foreach(var item in accDetails)
                    {
                        QueueCreateAccount.Enqueue(item);
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private CreateAccountInfo RetriveFromQueue()
        {
            CreateAccountInfo createAccountInfo = null;
            try
            {
                lock (Lock)
                {
                    createAccountInfo = QueueCreateAccount.Dequeue();
                }
            }
            catch (Exception)
            {
                // ignored
            }

            return createAccountInfo;
        }
    }
}
