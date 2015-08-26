using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kukkii.Core
{
    /// <summary>
    /// A basic thread runner thingy.
    /// It is responsible for all of the asynchronous operations running inside of Kukkii.
    /// </summary>
    public class CookieMonster : IDisposable
    {
        private ConcurrentQueue<Tuple<Func<object>, TaskCompletionSource<object>>> workQueue = null;
        private Task workerThread = null;
        private ManualResetEvent threadResetEvent = null;
        private CancellationTokenSource cancelSource = null;
        private CancellationToken cancelToken = CancellationToken.None;
        private volatile bool isInitialized = false;
        internal CookieMonster()
        {
            Initialize();
        }

        private void Initialize()
        {
            if (isInitialized) return;

            workQueue = new ConcurrentQueue<Tuple<Func<object>, TaskCompletionSource<object>>>();
            cancelSource = new CancellationTokenSource();
            cancelToken = cancelSource.Token;
            workerThread = new Task(RunQueue, cancelToken);
            threadResetEvent = new ManualResetEvent(false);
            workerThread.Start();

            isInitialized = true;
        }
        internal async Task DeinitializeAsync()
        {
            if (!isInitialized) return;

            cancelSource.Cancel();
            await workerThread;
            threadResetEvent.Dispose();

            if (workQueue.Count > 0)
            {
                while (workQueue.Count > 0)
                {
                    Tuple<Func<object>, TaskCompletionSource<object>> information = null;

                    if (workQueue.TryDequeue(out information))
                    {
                        Func<object> job = information.Item1;
                        TaskCompletionSource<object> reportingTask = information.Item2;

                        reportingTask.SetCanceled();
                    }
                }
            }

            isInitialized = false;
        }

        internal Task<object> QueueWork(Func<object> action)
        {
            var tcs = new TaskCompletionSource<object>();

            if (workerThread.IsFaulted || workerThread.IsCompleted)
            {
                workerThread = new Task(RunQueue, cancelToken);
                workerThread.Start();
            }


            threadResetEvent.Set();
            workQueue.Enqueue(new Tuple<Func<object>, TaskCompletionSource<object>>(action, tcs));

            return tcs.Task;
        }

        private void RunQueue()
        {
            while (!cancelToken.IsCancellationRequested)
            {
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
                                reportingTask.SetResult(job());
                            }
                            catch (Exception ex)
                            {
                                reportingTask.SetException(ex);
                            }
                        }
                    }

                    threadResetEvent.Reset();
                }
                else
                {
                    if (threadResetEvent.WaitOne(7000) == false)
                    {
                        if (workQueue.Count == 0)
                        {
                            //exit the thread. on phones, this was stealing an entire CPU core.
                            return;
                        }
                    }
                }
            }
        }
        public void Dispose()
        {
            DeinitializeAsync().Wait();
        }
    }
}