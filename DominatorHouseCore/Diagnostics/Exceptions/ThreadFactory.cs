#region

using System;
using System.Threading;
using System.Threading.Tasks;

#endregion

namespace DominatorHouseCore.Diagnostics
{
    /// <summary>
    ///     Class uses to create tasks with exception handlers
    /// </summary>
    public class ThreadFactory
    {
        public delegate void TaskError(Task task, Exception error);

        public static readonly ThreadFactory Instance = new ThreadFactory();

        private ThreadFactory()
        {
            Error += (t, e) =>
            {
                //GlobusExceptionHandler.HandleGlobalException(e, t.ToString()); 
                e.DebugLog();
            };
        }

        private event TaskError Error;

        private void InvokeError(Task task, Exception error)
        {
            Error?.Invoke(task, error);
        }

        public Task Start(Action action)
        {
            var task = new Task(action);
            Start(task);
            return task;
        }

        public Task Start(Action action, CancellationToken token,
            TaskCreationOptions options = TaskCreationOptions.LongRunning)
        {
            var task = new Task(action, token, options);
            Start(task);
            return task;
        }

        private void Start(Task task)
        {
            try
            {
                task.ContinueWith(t => InvokeError(t, t.Exception?.InnerException),
                    TaskContinuationOptions.OnlyOnFaulted |
                    TaskContinuationOptions.ExecuteSynchronously);
                task.Start();
            }
            catch (InvalidOperationException ex)
            {
                ex.DebugLog();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        //public Task<T> Start<T>(Func<T> action, TaskCreationOptions options)
        //{

        //    var task = new Task<T>(action, options);

        //    task.ContinueWith(t => InvokeError(t, t.Exception.InnerException),
        //                        TaskContinuationOptions.OnlyOnFaulted |
        //                        TaskContinuationOptions.ExecuteSynchronously);

        //    task.Start();

        //    return task;
        //}
    }
}