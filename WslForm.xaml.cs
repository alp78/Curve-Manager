using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Hosting;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace wsm
{
    public partial class WslForm : Page
    {
        public static string today = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local).ToString("yyyyMMdd");

        public static IDictionary<int, string> CurProcDict { get; set; }

        public static int lineCount = 0;
        public static StringBuilder output = new StringBuilder();

        public readonly Dispatcher dispatcher = Application.Current.MainWindow.Dispatcher;

        public List<string> ListeningCountryCodes = new List<string>();

        public List<Process> Processes { get; set; } = new List<Process>();
        public List<int> ProcessesIds { get; set; } = new List<int>();

        static int timeout = 28800;
        static int duration = 90;
        static string arguments = "";

        private void ReadPythonLog(object sender, EventArgs e)
        {
            foreach (KeyValuePair<string,string> country in SharedVariables.Countries)
            {
                string output_text;
                string country_code = country.Key;
                string filePath = $@"{SharedVariables.LogsFolder}\{country_code}_{today}.txt";

                if (File.Exists(filePath))
                {
                    try
                    {
                        output_text = File.ReadAllText(filePath);

                        switch (country_code)
                        {
                            case "AT":
                                AT.Text = output_text;
                                AT.ScrollToEnd();
                                break;
                            case "BE":
                                BE.Text = output_text;
                                BE.ScrollToEnd();
                                break;
                            case "CH":
                                CH.Text = output_text;
                                CH.ScrollToEnd();
                                break;
                            case "DE":
                                DE.Text = output_text;
                                DE.ScrollToEnd();
                                break;
                            case "DK":
                                DK.Text = output_text;
                                DK.ScrollToEnd();
                                break;
                            case "ES":
                                ES.Text = output_text;
                                ES.ScrollToEnd();
                                break;
                            case "FR":
                                FR.Text = output_text;
                                FR.ScrollToEnd();
                                break;
                            case "IT":
                                IT.Text = output_text;
                                IT.ScrollToEnd();
                                break;
                            case "NL":
                                NL.Text = output_text;
                                NL.ScrollToEnd();
                                break;
                            case "SE":
                                SE.Text = output_text;
                                SE.ScrollToEnd();
                                break;
                            case "NO":
                                NO.Text = output_text;
                                NO.ScrollToEnd();
                                break;
                            case "UK":
                                UK.Text = output_text;
                                UK.ScrollToEnd();
                                break;

                            default:
                                break;
                        }
                    }
                    catch (Exception)
                    {

                    }
                }
            }
        }

        public WslForm()
        {
            this.DataContext = this;
            InitializeComponent();

            DirectoryInfo log_dir = new DirectoryInfo(SharedVariables.LogsFolder);

            foreach (FileInfo log_file in log_dir.EnumerateFiles())
            {
                log_file.Delete();
            }

            CurProcDict = new Dictionary<int, string>();
            CountryListBox.ItemsSource = SharedVariables.CountryList;

            Duration.IsEnabled = false;
            DurationLabel.IsEnabled = false;

            DispatcherTimer timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3) };
            timer.Tick += ReadPythonLog;
            timer.Start();
        }

        private void CountryListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            CountryListBox.UnselectAll();
        }

        private void LogWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            
            Dispatcher.Invoke(new Action(() =>

            {
                string country_code = e.FullPath.Substring(e.FullPath.Length - 15, 2);
                string filePath = $@"{SharedVariables.LogsFolder}\{country_code}_{today}.txt";

                try
                {
                    string firstline = File.ReadAllText(filePath);
                    foreach (TextBox CurrentTextBox in MonitorGrid.Children.OfType<TextBox>())
                    {
                        if (CurrentTextBox.Name == country_code)
                        {
                            CurrentTextBox.Text = ($"{firstline}");
                            CurrentTextBox.ScrollToEnd();
                            break;
                        }
                    }
                }
                catch (IOException)
                {
 
                }
            }));
        }


        private void StartListen_Click(object sender, RoutedEventArgs e)
        {
            Processes.Clear();

            List<string> SelectedCountries = new List<string>();

            foreach (string country in CountryListBox.SelectedItems)
            {
                SelectedCountries.Add(country);
            }

            string country_list_string = string.Join("\r\n", SelectedCountries);

            MessageBoxResult messageBoxResult = MessageBox.Show($"Are you sure?\r\n\r\n{country_list_string}", "WSL Launch", MessageBoxButton.YesNo);

            if (messageBoxResult == MessageBoxResult.Yes)
            {

                bool args_ok = true;

                if (String.IsNullOrEmpty(Timeout.Text) | Timeout.Text == "28800")
                {
                    timeout = 28800;
                }
                else
                {
                    try
                    {
                        timeout = Int32.Parse(Timeout.Text);
                    }
                    catch (Exception)
                    {
                        Processes.Clear();
                        InitForecast.IsChecked = false;
                        CountryListBox.UnselectAll();
                        SelectedCountries.Clear();
                        Timeout.Text = string.Empty;
                        args_ok = false;
                        MessageBox.Show("Timeout must be an integer!");
                    }
                }

                if (InitForecast.IsChecked == true)
                {
                    if (String.IsNullOrEmpty(Duration.Text) | Duration.Text == "90")
                    {
                        duration = 90;
                    }
                    else
                    {
                        try
                        {
                            duration = Int32.Parse(Duration.Text);
                        }
                        catch (Exception)
                        {
                            Processes.Clear();
                            InitForecast.IsChecked = false;
                            CountryListBox.UnselectAll();
                            SelectedCountries.Clear();
                            Duration.Text = string.Empty;
                            args_ok = false;
                            MessageBox.Show("Duration must be an integer!");
                        }
                    }
                }

                if (args_ok == true)
                {
                    string listening_country_code;

                    StopListen.IsEnabled = true;

                    foreach (string selected_country in SelectedCountries)
                    {
                        int i = 0;

                        foreach (string country in SharedVariables.CountryList)
                        {

                            if (country == selected_country)
                            {

                                listening_country_code = SharedVariables.Countries.Single(x => x.Value == country).Key;

                                string wsl_script;

                                if (InitForecast.IsChecked == true & InitHistoric.IsChecked == false)
                                {
                                    wsl_script = $@"C:\Users\{SharedVariables.User}\WATTSIGHT\PYLIB\LISTEN\wslpd.py";
                                    arguments = $"{wsl_script} {listening_country_code} {timeout} {duration}";
                                }
                                else if (InitForecast.IsChecked == true & InitHistoric.IsChecked == true)
                                {
                                    wsl_script = $@"C:\Users\{SharedVariables.User}\WATTSIGHT\PYLIB\LISTEN\wslpdh.py";
                                    arguments = $"{wsl_script} {listening_country_code} {timeout} {duration}";
                                }
                                else if (InitForecast.IsChecked == false & InitHistoric.IsChecked == true)
                                {
                                    wsl_script = $@"C:\Users\{SharedVariables.User}\WATTSIGHT\PYLIB\LISTEN\wslph.py";
                                    arguments = $"{wsl_script} {listening_country_code} {timeout}";
                                }
                                else
                                {
                                    wsl_script = $@"C:\Users\{SharedVariables.User}\WATTSIGHT\PYLIB\LISTEN\wslpf.py";
                                    arguments = $"{wsl_script} {listening_country_code} {timeout}";
                                }

                                string filePath = $@"C:\Users\{SharedVariables.User}\WATTSIGHT\WSLOGS\{listening_country_code.ToUpper()}_{today}.txt";

                                var listBoxItem = CountryListBox.ItemContainerGenerator.ContainerFromIndex(i);
                                (listBoxItem as ListBoxItem).IsEnabled = false;

                                Process process = new Process();

                                ProcessStartInfo startInfo = new ProcessStartInfo
                                {
                                    WindowStyle = ProcessWindowStyle.Hidden,
                                    FileName = $@"C:\Users\{SharedVariables.User}\AppData\Local\Programs\Python\Python38\python.exe",
                                    Arguments = arguments,
                                    CreateNoWindow = true,
                                    RedirectStandardOutput = true,
                                    UseShellExecute = false
                                };

                                process.StartInfo = startInfo;

                                process.Start();
                                Processes.Add(process);

                                int proc_id = process.Id;
                                string cc_up = listening_country_code.ToUpper();

                                CurProcDict.Add(new KeyValuePair<int, string>(proc_id, cc_up));
                                break;
                            }
                            i++;
                        }
                    }
                }
            }
            else
            {
                CountryListBox.UnselectAll();
            }
            InitForecast.IsChecked = false;
            InitHistoric.IsChecked = false;
            CountryListBox.UnselectAll();
            SelectedCountries.Clear();
        }

        private void StopListen_Click(object sender, RoutedEventArgs e)
        {
            int i = 0;
            foreach (string country in SharedVariables.CountryList)
            {
                var listBoxItem = CountryListBox.ItemContainerGenerator.ContainerFromIndex(i);
                (listBoxItem as ListBoxItem).IsEnabled = true;
                i++;
            }

            Process[] python_array = Process.GetProcessesByName("Python");

            foreach (Process python in python_array)
            {
                python.Kill();
            }

            foreach (Process proc in Processes)
            {
                if (!proc.HasExited)
                {
                    try
                    {
                        proc.CloseMainWindow();
                    }
                    catch (Exception)
                    {

                    }
                    proc.Kill();
                }
            }

            CurProcDict = new Dictionary<int, string>();

            Processes.Clear();

            AT.Text = string.Empty;
            BE.Text = string.Empty;
            CH.Text = string.Empty;
            DE.Text = string.Empty;
            DK.Text = string.Empty;
            ES.Text = string.Empty;
            FR.Text = string.Empty;
            IT.Text = string.Empty;
            NL.Text = string.Empty;
            NO.Text = string.Empty;
            SE.Text = string.Empty;
            UK.Text = string.Empty;

            DirectoryInfo log_dir = new DirectoryInfo(SharedVariables.LogsFolder);

            foreach (FileInfo log_file in log_dir.EnumerateFiles())
            {
                log_file.Delete();
            }

        }

        private void InitForecast_Checked(object sender, RoutedEventArgs e)
        {
            DurationLabel.IsEnabled = true;
            Duration.IsEnabled = true;
        }

        private void InitForecast_Unchecked(object sender, RoutedEventArgs e)
        {
            DurationLabel.IsEnabled = false;
            Duration.IsEnabled = false;
        }

        private void InitHistoric_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void InitHistoric_Unchecked(object sender, RoutedEventArgs e)
        {

        }
    }
}
