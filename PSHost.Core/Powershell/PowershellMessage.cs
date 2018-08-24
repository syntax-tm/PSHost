using System;
using JetBrains.Annotations;

namespace PSHost.Core.Powershell
{
    public class PowershellMessage
    {
        /// <summary>
        /// The text content displayed in the <seealso cref="OutputStream"/>.
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// The <c>Output</c> stream containing the <seealso cref="Message"/>.
        /// </summary>
        public OutputStream OutputStream { get; set; }
        /// <summary>
        /// The <seealso cref="DateTime"/> the message was shown (or created).
        /// </summary>
        public DateTime CreatedOn { get; set; } = DateTime.Now;

        public PowershellMessage([NotNull] string message, OutputStream outputStream = OutputStream.None)
        {
            Message = message;
            OutputStream = outputStream;
        }
    }
}
