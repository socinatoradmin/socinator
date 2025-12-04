using System;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using QuoraDominatorCore.QdLibrary;
using DominatorHouseCore.BusinessLogic.Scraper;
using QuoraDominatorCore.Factories;

namespace QuoraDominatorCore.Library
{
    public class QdJobProcessFactory : IJobProcessFactory
    {
        static QdJobProcessFactory _instance;

        public static QdJobProcessFactory Instance => _instance ?? (_instance = new QdJobProcessFactory());


        private QdJobProcessFactory()
        {
            //DominatorJobProcessFactory.QdAccountConfigScheduler += AccountConfigScheduler;
            //DominatorScraperFactory.QdAccountConfigScraper += AccountConfigScraper;
        }    // singleton

        public JobProcess AccountConfigScheduler(string account, string template, TimingRange timings, string module)
            => QdJobProcessFactory.Instance.Create(account, template, timings, module,SocialNetworks.Quora);

        //public AbstractQueryScraper AccountConfigScraper(JobProcess jobProcess)
        //    => QDQueryScraperFactory.Instance.Create(jobProcess);

        public JobProcess Create(string account, string template, TimingRange currentJobTimeRange, string module, SocialNetworks network)
        {
            ActivityType activity = (ActivityType)Enum.Parse(typeof(ActivityType), module);

            // TODO: remove unused activities
            // Add creation of activity objects that was derived from JobProcess
            switch (activity)
            {
                case ActivityType.Follow:
                    FollowProcess ObjFollowProcess = new FollowProcess(account, template, activity, currentJobTimeRange);
                    return ObjFollowProcess;

                case ActivityType.Unfollow:
                    UnfollowProcess ObjUnfollowProcess = new UnfollowProcess(account, template, activity, currentJobTimeRange);
                    return ObjUnfollowProcess;

                case ActivityType.UpvoteAnswers:
                    UpvoteProcess ObjUpvoteProcess = new UpvoteProcess(account, template, activity, currentJobTimeRange);
                    return ObjUpvoteProcess;

                case ActivityType.DownvoteAnswers:
                    DownvoteProcess ObjDownvoteProcess = new DownvoteProcess(account, template, activity, currentJobTimeRange);
                    return ObjDownvoteProcess;

                case ActivityType.DownvoteQuestions:
                    return new DownvoteQuestionProcess(account, template, activity, currentJobTimeRange);

                case ActivityType.ReportAnswers:
                    return new ReportAnswerProcess(account, template, activity, currentJobTimeRange);

                case ActivityType.AnswersScraper:
                    return new AnswerScraperProcess(account, template, activity, currentJobTimeRange);

                case ActivityType.QuestionsScraper:
                    return new QuestionScraperProcess(account, template, activity, currentJobTimeRange);

                case ActivityType.DeletePost:
                    throw new NotImplementedException();

                case ActivityType.Message:
                    throw new NotImplementedException();

                case ActivityType.UserScraper:
                    return new UserScraperProcess(account, template, activity, currentJobTimeRange);

                case ActivityType.ReportUsers:
                    ReportUser ObjReportUser = new ReportUser(account, template, activity, currentJobTimeRange);
                    return ObjReportUser;

                case ActivityType.Reposter:
                    throw new NotImplementedException();

                default:
                    throw new ArgumentException(module);
            }
        }

        //private QdJobProcessFactory() { }    // singleton

        //public JobProcess Create(string account, string template, TimingRange currentJobTimeRange, string module,
        //    SocialNetworks network)
        //{
            
        //}
    }
}
