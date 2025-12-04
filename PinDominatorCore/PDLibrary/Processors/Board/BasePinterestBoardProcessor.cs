using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using PinDominatorCore.PDLibrary.DAL;
using PinDominatorCore.PDModel;
using PinDominatorCore.Response;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PinDominatorCore.PDLibrary.Processors.Board
{
    public abstract class BasePinterestBoardProcessor : BasePinterestProcessor
    {
        private static readonly Queue<KeyValuePair<string, BoardInfo>> QueueCreateBoard = new Queue<KeyValuePair<string, BoardInfo>>();
        private static readonly object LockObject = new object();

        protected BasePinterestBoardProcessor(IPdJobProcess jobProcess, IDbGlobalService globalService,
            IDbCampaignService campaignService, IPinFunction objPinFunct) :
            base(jobProcess, globalService, campaignService, objPinFunct)
        {
        }

        /// <summary>
        /// Here we are it iterating list of PinterestBoard in foreach loop where we will send each to StartCustomBoardProcess
        /// </summary>
        /// <param name="queryInfo"></param>
        /// <param name="jobProcessResult"></param>
        /// <param name="lstPinBoards"></param>
        public void StartProcessForListOfBoards(QueryInfo queryInfo, ref JobProcessResult jobProcessResult, List<PinterestBoard> lstPinBoards)
        {
            if (lstPinBoards.Count <= 0)
                return;
            var newPinterestBoard = lstPinBoards.GetRandomItem();
            if (ActivityType != ActivityType.AcceptBoardInvitation && ActivityType != ActivityType.SendBoardInvitation)
            {
                if (FilterUserActionList.Count > 0)
                {
                    UserNameInfoPtResponseHandler pinterestUser;
                    pinterestUser = PinFunction.GetUserDetails(newPinterestBoard.UserName, JobProcess.DominatorAccountModel).Result;

                    var boards = DbAccountService.GetScrapBoardsWithSameQuery(ActivityType + "_Scrap", queryInfo).
                        FirstOrDefault(x => x.Username == newPinterestBoard.UserName);
                    if (FilterUserApply(pinterestUser, 1))
                    {
                        lstPinBoards.Clear();
                        if (boards != null)
                        {
                            boards.Filtered = true;
                            DbAccountService.Update(boards);
                        }
                        return;
                    }
                }
                if (FilterPinActionList.Count > 0)
                {
                    //Implement Pin Filter for Custom Board.
                    List<PinterestPin> PinList;
                    if (JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                        PinList = BrowserManager.SearchPinsOfBoard(JobProcess.DominatorAccountModel, queryInfo.QueryValue, JobProcess.DominatorAccountModel.CancellationSource);
                    else
                        PinList = PinFunction.GetPinsByBoardUrl(queryInfo.QueryValue, JobProcess.DominatorAccountModel).LstBoardPin;
                    foreach (var Pin in PinList)
                        if (FilterPinApply(Pin, 1))
                            return;
                }
            }
            foreach (var board in lstPinBoards)
                try
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    if (board.IsFollowed)
                        continue;

                    if (JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser && ActivityType != ActivityType.BoardScraper &&
                        ActivityType != ActivityType.SendBoardInvitation)
                        BrowserManager.AddNew(JobProcess.DominatorAccountModel.CancellationSource, $"https://{BrowserManager.Domain}");

                    StartCustomBoardProcess(queryInfo, ref jobProcessResult, board);
                }
                catch (OperationCanceledException)
                {
                    throw new OperationCanceledException();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
                finally
                {
                    if (JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser && BrowserManager.BrowserWindows.Count > 1)
                        BrowserManager.CloseLast();
                }
        }

        /// <summary>
        /// Here we are filtering boards according to filters applied than we will send it to final process 
        /// </summary>
        /// <param name="queryInfo"></param>
        /// <param name="jobProcessResult"></param>
        /// <param name="newPinterestBoard"></param>
        public void StartCustomBoardProcess(QueryInfo queryInfo, ref JobProcessResult jobProcessResult, PinterestBoard newPinterestBoard)
        {
            try
            {
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                if (ActivityType != ActivityType.SendBoardInvitation)
                {
                    if (AlreadyInteractedBoard(queryInfo, newPinterestBoard.BoardUrl)) return;
                }

                if (ActivityType != ActivityType.AcceptBoardInvitation && ActivityType != ActivityType.SendBoardInvitation)
                {
                    if (JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                        newPinterestBoard = BrowserManager.SearchByCustomBoard(JobProcess.DominatorAccountModel, newPinterestBoard.BoardUrl, JobProcess.DominatorAccountModel.CancellationSource);
                    else
                        newPinterestBoard = PinFunction.GetBoardDetails(newPinterestBoard.BoardUrl, JobProcess.DominatorAccountModel);
                }
                StartFinalProcess(ref jobProcessResult, newPinterestBoard, queryInfo);
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        /// <summary>
        /// Here we will send board details and query info to final process where activity will be performed.
        /// </summary>
        /// <param name="jobProcessResult"></param>
        /// <param name="newPinterestBoard"></param>
        /// <param name="queryInfo"></param>
        public void StartFinalProcess(ref JobProcessResult jobProcessResult, PinterestBoard newPinterestBoard,
            QueryInfo queryInfo)
        {
            jobProcessResult = JobProcess.FinalProcess(new ScrapeResultNew
            {
                ResultPost = newPinterestBoard,
                QueryInfo = queryInfo
            });
        }

        /// <summary>
        /// Here we are it iterating list of BoardInfo in foreach loop where we will send each to 
        /// StartFinalProcessForCreateBoard.
        /// </summary>
        /// <param name="jobProcessResult"></param>
        /// <param name="boardsList"></param>
        public void StartProcessForListOfBoardsToCreate(ref JobProcessResult jobProcessResult,
            List<BoardInfo> boardsList)
        {
            try
            {
                var boardModel = JsonConvert.DeserializeObject<BoardModel>(TemplateModel.ActivitySettings);

                if (boardModel.UniqueBoardWithCampaign)
                {
                    AddBoardDetailsToQueue(boardsList);

                    lock (LockObject)
                    {
                        while (QueueCreateBoard.Where(x => x.Key == JobProcess.CampaignId).ToList().Count > 0)
                        {
                            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                            var board = FetchBoardDetailsFromQueue();
                            StartFinalProcessForCreateBoard(ref jobProcessResult, board);
                        }
                    }
                }
                else
                {
                    foreach (var board in boardsList)
                    {
                        JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        if (AlreadyInteractedBoard(board))
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                String.Format("LangKeyAlreadyCreatedBoard".FromResourceDictionary(), board.BoardName));
                            continue;
                        }
                        StartFinalProcessForCreateBoard(ref jobProcessResult, board);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            jobProcessResult.IsProcessCompleted = true;
        }

        /// <summary>
        /// Here we will send board details to final process where activity will be performed.
        /// </summary>
        /// <param name="jobProcessResult"></param>
        /// <param name="boardInfo"></param>
        public void StartFinalProcessForCreateBoard(ref JobProcessResult jobProcessResult, BoardInfo boardInfo)
        {
            jobProcessResult = JobProcess.FinalProcess(new ScrapeResultNew
            {
                ResultPost = boardInfo
            });
        }

        private void AddBoardDetailsToQueue(List<BoardInfo> boardsList)
        {
            try
            {
                lock (LockObject)
                {
                    if (QueueCreateBoard.FirstOrDefault(x => x.Key == JobProcess.CampaignId).Value != null)
                        return;
                    var lstInteractedBoards = CampaignService.GetInteractedBoards(ActivityType).ToList();
                    foreach (var board in boardsList)
                    {
                        if (lstInteractedBoards.Any(x => x.BoardName.Equals(board.BoardName)))
                            continue;
                        QueueCreateBoard.Enqueue(new KeyValuePair<string, BoardInfo>(JobProcess.CampaignId, board));
                    }
                }
            }
            catch
            {
                // ignored
            }
        }

        private BoardInfo FetchBoardDetailsFromQueue()
        {
            try
            {
                lock (LockObject)
                {
                    return QueueCreateBoard.Dequeue().Value;
                }
            }
            catch (Exception) { return null; }
        }

    }
}