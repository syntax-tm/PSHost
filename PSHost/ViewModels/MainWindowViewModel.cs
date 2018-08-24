using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using log4net;
using PSHost.Core.Powershell;

namespace PSHost.ViewModels
{
    [POCOViewModel]
    public class MainWindowViewModel
    {
        protected readonly ILog log = LogManager.GetLogger(nameof(MainWindowViewModel));

        public virtual bool IsBusy { get; set; }
        public virtual bool OutString { get; set; } = true;
        public virtual string CommandText { get; set; } = @"Get-Process chrome";
        
        public virtual int? CommandHistoryIndex { get; set; }
        public virtual ObservableCollection<string> CommandHistory { get; set; } = new ObservableCollection<string>();
        public virtual ObservableCollection<PowershellResult> Results { get; set; } = new ObservableCollection<PowershellResult>();
        public virtual ObservableCollection<PowershellMessage> Messages { get; set; } = new ObservableCollection<PowershellMessage>();

        protected MainWindowViewModel()
        {
            
        }

        public static MainWindowViewModel Create()
        {
            return ViewModelSource.Create(() => new MainWindowViewModel());
        }

        public void Execute()
        {
            if (string.IsNullOrWhiteSpace(CommandText)) return;

            try
            {
                IsBusy = true;

                Messages.Add(new PowershellMessage(CommandText, OutputStream.Command));

                CommandHistory.Add(CommandText);

                using (var host = new Core.Powershell.PSHost())
                {
                    var result = new PowershellResult(CommandText);
                    var response = host.Execute(CommandText, OutString);

                    if (response == null || !response.Any())
                    {
                        var noResults = @"Command returned no results...";
                        result.OnCompleted(noResults);
                        Results.Add(result);
                        Messages.Add(new PowershellMessage(noResults, OutputStream.Warning));
                        Clear();
                        return;
                    }

                    var responseSb = new StringBuilder();

                    foreach (var obj in response)
                    {
                        var objectSb = new StringBuilder();

                        if (OutString)
                        {
                            var lines = obj.ToString().Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                            var output = lines.Select(l => l.TrimEnd());
                            var formatted = string.Join(Environment.NewLine, output).Trim();

                            objectSb.Append(formatted);
                        }
                        else
                        {
                            objectSb.AppendLine($"{obj.BaseObject.GetType().FullName}");

                            foreach (var property in obj.Properties)
                            {
                                objectSb.Append($"{property.Name}: {property.Value}");
                            }
                        }

                        responseSb.AppendLine(objectSb.ToString());

                        Messages.Add(new PowershellMessage(objectSb.ToString()));
                    }

                    result.OnCompleted(responseSb.ToString().Trim());

                    Results.Add(result);

                    Clear();
                }
            }
            catch (Exception e)
            {
                var message = $"An error occurred executing the Powershell command. {e.Message}";
                log.Error(message, e);
                MessageBox.Show(message, "Powershell Execution Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public void Clear()
        {
            CommandText = string.Empty;
            CommandHistoryIndex = null;
        }

        public void Reset()
        {
            Clear();
            CommandHistory.Clear();
            Results.Clear();
        }

        public void Previous()
        {
            var maxIndex = CommandHistory.Count - 1;
            var index = CommandHistoryIndex ?? maxIndex;
            var nextCommand = Math.Min(0, index);
            var command = CommandHistory[nextCommand];

            CommandText = command;
        }

        public void Next()
        {
            //  we're not even navigating the journal/index of previous
            //  commands, so there's nothing newer in this case
            if (CommandHistoryIndex == null)
            {
                Clear();
                return;
            }

            //  we're already on the newest/most-recent item, so
            //  clear the text and return
            var maxIndex = CommandHistory.Count - 1;
            if (CommandHistoryIndex == maxIndex)
            {
                
                Clear();
                return;
            }

            var index = CommandHistoryIndex ?? maxIndex;
            var nextCommand = Math.Max(maxIndex, index);
            var command = CommandHistory[nextCommand];

            CommandText = command;
        }

    }
}
