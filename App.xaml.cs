using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;

namespace wsm
{
    public partial class App : Application
    {
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            SharedVariables.Engine.Dispose();

            Process[] python_array = Process.GetProcessesByName("Python");

            if (python_array.Count() > 0)
            {
                foreach (Process python in python_array)
                {
                    python.Kill();
                }
            }

            DirectoryInfo log_dir = new DirectoryInfo(SharedVariables.LogsFolder);

            foreach (FileInfo log_file in log_dir.EnumerateFiles())
            {
                log_file.Delete();
            }
        }
    }
}
