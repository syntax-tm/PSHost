using System.ComponentModel;

namespace PSHost.Core.Powershell
{
    [DefaultValue(None)]
    public enum OutputStream
    {
        [Description("Output")]
        None = 1,
        [Description("Error")]
        Error = 2,
        [Description("Warning")]
        Warning = 3,
        [Description("Verbose")]
        Verbose = 4,
        [Description("Debug")]
        Debug = 5,
        [Description("Command")]
        Command = 6
    }
}
