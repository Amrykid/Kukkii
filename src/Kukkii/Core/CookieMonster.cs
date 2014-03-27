using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace Kukkii.Core
{
    /// <summary>
    /// A basic thread runner thingy.
    /// It is responsible for all of the asynchronous operations running inside of Kukkii.
    /// </summary>
    internal static class CookieMonster
    {
        private static ConcurrentQueue<Tuple<Action, TaskCompletionSource<bool>>> workQueue = null;
        private static Task workerThread = null;
        private static ManualResetEvent threadResetEvent = null;
        static CookieMonster()
        {
            workQueue = new ConcurrentQueue<Tuple<Action, TaskCompletionSource<bool>>>();
            workerThread = new Task(RunQueue);
            threadResetEvent = new ManualResetEvent(false);
        }

        internal static Task<bool> QueueWork(Action action)
        {
            var tcs = new TaskCompletionSource<bool>();

            workQueue.Enqueue(new Tuple<Action, TaskCompletionSource<bool>>(action, tcs));

            if (workerThread.Status != TaskStatus.Running)
                workerThread.Start();

            return tcs.Task;
        }

        private static void RunQueue()
        {
            threadResetEvent.Reset();

            if (workQueue.Count > 0)
            {
                while (workQueue.Count > 0)
                {
                    Tuple<Action, TaskCompletionSource<bool>> information = null;

                    if (workQueue.TryDequeue(out information))
                    {
                        Action job = information.Item1;
                        TaskCompletionSource<bool> reportingTask = information.Item2;

                        try
                        {
                            job();
                            reportingTask.TrySetResult(true);
                        }
                        catch (Exception ex)
                        {
                            reportingTask.TrySetException(ex);
                        }
                    }
                }
            }
            else
            {
                threadResetEvent.WaitOne(300);
            }

            RunQueue();
        }
    }
}
