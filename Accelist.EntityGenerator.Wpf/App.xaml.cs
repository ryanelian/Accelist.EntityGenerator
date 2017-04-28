using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Accelist.EntityGenerator.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            File.WriteAllText("error.log", SerializeException(e.Exception));
            MessageBox.Show($"Logged to error.log.", "An unhandled exception has occurred!", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }

        public string SerializeException(Exception ex)
        {
            var sb = new StringBuilder();
            sb.AppendLine("An unhandled exception has occurred!");
            sb.Append(ex.GetType().ToString());
            sb.Append(": ");
            sb.AppendLine(ex.Message);
            sb.Append(ex.StackTrace);

            if (ex is AggregateException)
            {
                var ag = (AggregateException)ex;
                var exs = ag.Flatten().InnerExceptions;
                for (var i = 0; i < exs.Count; i++)
                {
                    var exi = exs[i];

                    sb.Append("\n\n");
                    sb.Append($"---> (Inner Exception #{i}) ");
                    sb.Append(exi.GetType().ToString());
                    sb.Append(": ");
                    sb.AppendLine(exi.Message);
                    sb.Append(exi.StackTrace);
                }
            }

            return sb.ToString();
        }
    }


}
