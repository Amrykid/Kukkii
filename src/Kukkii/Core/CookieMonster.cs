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
        private static ConcurrentQueue<Tuple<Func<object>, TaskCompletionSource<object>>> workQueue = null;
        private static Task workerThread = null;
        private static ManualResetEvent threadResetEvent = null;
        static CookieMonster()
        {
            workQueue = new ConcurrentQueue<Tuple<Func<object>, TaskCompletionSource<object>>>();
            workerThread = new Task(RunQueue);
            threadResetEvent = new ManualResetEvent(false);
        }

        internal static Task<object> QueueWork(Func<object> action)
        {
            var tcs = new TaskCompletionSource<object>();

            workQueue.Enqueue(new Tuple<Func<object>, TaskCompletionSource<object>>(action, tcs));

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
                    Tuple<Func<object>, TaskCompletionSource<object>> information = null;

                    if (workQueue.TryDequeue(out information))
                    {
                        Func<object> job = information.Item1;
                        TaskCompletionSource<object> reportingTask = information.Item2;

                        try
                        {
                            reportingTask.TrySetResult(job());
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
