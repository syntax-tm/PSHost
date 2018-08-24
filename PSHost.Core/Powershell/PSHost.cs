using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using JetBrains.Annotations;
using log4net;

namespace PSHost.Core.Powershell
{
    public class PSHost : IDisposable
    {

        protected readonly ILog log = LogManager.GetLogger(nameof(PSHost));

        private bool _disposed = false;

        public PSHost()
        {

        }

        public IList<PSObject> Execute([NotNull] string commandText, bool outString = true)
        {
            try
            {
                using (var powershell = PowerShell.Create())
                {
                    powershell.AddScript(commandText);

                    if (outString)
                    {
                        powershell.AddCommand("Out-String");
                    }

                    var output = powershell.Invoke();
                    
                    var errors = powershell.Streams.Error;
                    if (!errors.Any()) return output.ToList();

                    var messageSb = new StringBuilder();

                    messageSb.AppendLine(@"One or more errors occurred after invoking the Powershell host. See below for details.");
                    messageSb.AppendLine();

                    var i = 0;
                    foreach (var error in errors)
                    {
                        var errorSb = new StringBuilder();

                        errorSb.AppendLine($@"Error {i} {error.ErrorDetails}");
                        errorSb.AppendLine($@"Error ID:{Environment.NewLine}{error.FullyQualifiedErrorId}");
                        errorSb.AppendLine($@"Message:{Environment.NewLine}{error.Exception?.Message}");
                        errorSb.AppendLine($@"StackTrace:{Environment.NewLine}{error.ScriptStackTrace}");

                        log.Error(errorSb, error.Exception);

                        messageSb.AppendLine(errorSb.ToString());
                        messageSb.AppendLine();
                        i++;
                    }
                    
                    throw new CmdletInvocationException(messageSb.ToString());
                }
            }
            catch (Exception e)
            {
                var messageSb = new StringBuilder();
                messageSb.AppendLine($"Error executing Powershell script:{Environment.NewLine}{commandText}");
                messageSb.AppendLine($"{Environment.NewLine}{Environment.NewLine}{e.Message}");
                log.Error(messageSb, e);
                return null;
            }
        }
        
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                //  TODO: free up managed resources
            }

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
