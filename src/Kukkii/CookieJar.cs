using Kukkii.Containers;
using Kukkii.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kukkii
{
    public static class CookieJar
    {
        private static TaskCompletionSource<object> initializationTask = new TaskCompletionSource<object>();

        static CookieJar()
        {
            //initialize the cookie jar!
            InMemory = new BasicCookieContainer();
            Device = new PersistentCookieContainer();

            initializationTask.TrySetResult(true);
        }

        public static Task EnsureInitializedAsync()
        {
            return initializationTask.Task;
        }

        public static string ApplicationName { get; set; }
        public static ICookieContainer InMemory { get; private set; }
        public static ICookieContainer Device { get; private set; }
    }
}
