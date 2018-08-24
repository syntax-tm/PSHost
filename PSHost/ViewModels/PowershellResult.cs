using System;
using System.Threading;
using JetBrains.Annotations;

namespace PSHost.ViewModels
{
    public class PowershellResult
    {
        private static int CommandResultCount;
        private static readonly object syncLock = new object();
        
        public int CommandId { get; }
        public DateTime StartedOn { get; set; } = DateTime.Now;
        public DateTime CompletedOn { get; set; }
        public TimeSpan Elapsed => (CompletedOn - StartedOn);
        public string Command { get; set; }
        public string Results { get; set; }
        public Exception Exception { get; set; }
        public bool IsSuccess => Exception == null;
        public bool IsFailure => !IsSuccess;

        public PowershellResult([NotNull] string command)
        {
            lock (syncLock)
            {
                Interlocked.Increment(ref CommandResultCount);
                CommandId = CommandResultCount;
            }
            Command = command;
        }

        public void OnCompleted([NotNull] Exception exception)
        {
            Exception = exception;
            Results = exception.Message;
            CompletedOn = DateTime.Now;
        }

        public void OnCompleted([NotNull] string results)
        {
            Results = results.Trim();
            CompletedOn = DateTime.Now;
        }
    }
}
