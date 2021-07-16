using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Diagnostics;
using System.Security;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Net.Http;
using RDotNet;
using System.Windows;

namespace wsm
{
    [SuppressUnmanagedCodeSecurity]
    public static class ConsoleManager
    {
        private const string Kernel32_DllName = "kernel32.dll";

        [DllImport(Kernel32_DllName)]
        private static extern bool AllocConsole();

        [DllImport(Kernel32_DllName)]
        private static extern bool FreeConsole();

        [DllImport(Kernel32_DllName)]
        private static extern IntPtr GetConsoleWindow();

        [DllImport(Kernel32_DllName)]
        private static extern int GetConsoleOutputCP();

        public static bool HasConsole
        {
            get { return GetConsoleWindow() != IntPtr.Zero; }
        }

        public static void Show()
        {
            if (!HasConsole)
            {
                AllocConsole();
                InvalidateOutAndError();
            }
        }

        public static void Hide()
        {
            if (HasConsole)
            {
                SetOutAndErrorNull();
                FreeConsole();
            }
        }

        public static void Toggle()
        {
            if (HasConsole)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }

        static void InvalidateOutAndError()
        {
            Type type = typeof(System.Console);

            System.Reflection.FieldInfo _out = type.GetField("_out",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

            System.Reflection.FieldInfo _error = type.GetField("_error",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

            System.Reflection.MethodInfo _InitializeStdOutError = type.GetMethod("InitializeStdOutError",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

            Debug.Assert(_out != null);
            Debug.Assert(_error != null);

            Debug.Assert(_InitializeStdOutError != null);

            _out.SetValue(null, null);
            _error.SetValue(null, null);

            _InitializeStdOutError.Invoke(null, new object[] { true });
        }
        static void SetOutAndErrorNull()
        {
            Console.SetOut(TextWriter.Null);
            Console.SetError(TextWriter.Null);
        }
    }
    public static class ObservableCollection
    {
        public static void Sort<TSource, TKey>(this ObservableCollection<TSource> source, Func<TSource, TKey> keySelector, bool isAZ)
        {
            if (isAZ)
            {
                List<TSource> sortedList = source.OrderBy(keySelector).ToList();
                source.Clear();
                foreach (var sortedItem in sortedList)
                {
                    source.Add(sortedItem);
                }
            }
            else
            {
                List<TSource> sortedList = source.OrderByDescending(keySelector).ToList();
                source.Clear();
                foreach (var sortedItem in sortedList)
                {
                    source.Add(sortedItem);
                }
            }
        }
    }
    public class Curve
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Cluster { get; set; }
        public string State { get; set; } = "OK";
    }
    public class ForMeta
    {
        public string ForCurveId { get; set; }
        public string ForTableName { get; set; }
        public string ForCurveName { get; set; }
        public string ForCurveType { get; set; }
        public string ForDataType { get; set; }
        public string ForUnit { get; set; }
        public string ForFrequency { get; set; }
        public string ForFunction { get; set; }
        public string ForIssueDate { get; set; }
        public string ForCreated { get; set; }
        public string ForModified { get; set; }
        public string ForDataFrom { get; set; }
        public string ForDataTo { get; set; }
        public string ForDataSize { get; set; }
        public string Cluster { get; set; }
        public string State { get; set; } = "OK";
    }
    public class HisMeta
    {
        public string HisCurveId { get; set; }
        public string HisTableName { get; set; }
        public string HisCurveName { get; set; }
        public string HisCurveType { get; set; }
        public string HisDataType { get; set; }
        public string HisUnit { get; set; }
        public string HisFrequency { get; set; }
        public string HisFunction { get; set; }
        public string HisDataFrom { get; set; }
        public string HisDataTo { get; set; }
        public string HisDataSize { get; set; }
        public string Cluster { get; set; }
        public string State { get; set; } = "OK";
    }
    public class CountryBoxItem
    {
        public string Name { get; set; }
        public bool IsListening { get; set; }
        public string BoxName { get; set; }
    }
    public static class SharedVariables
    {
        private static string _user;

        private static string _logs_folder; 

        private static Dictionary<string, string> _countries;
        private static List<string> _country_list;
        private static string _country_code;
        private static string _country;

        private static ListCollectionView _forecast_view;
        private static ListCollectionView _historic_view;

        private static ListCollectionView _forecast_db_view;
        private static ListCollectionView _historic_db_view;

        private static string _forecast_json;
        private static string _historic_json;

        private static ObservableCollection<Curve> _forecast_list;
        private static ObservableCollection<Curve> _historic_list;

        private static ObservableCollection<ForMeta> _for_meta_list;
        private static ObservableCollection<HisMeta> _his_meta_list;

        private static JObject _secrets_json;
        private static string _secrets_path;

        private static ObservableCollection<CountryBoxItem> _listening_countries;

        private static Button _add_json_button;
        private static Button _remove_json_button;
        private static Button _create_table_button;
        private static Button _drop_table_button;

        private static REngine _engine;

        private static HttpClient _client = new HttpClient();

        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;


        public static string User
        {
            get { return _user; }
            set
            {
                _user = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs("CurrentUser"));
            }
        }

        public static string LogsFolder
        {
            get { return _logs_folder; }
            set
            {
                _logs_folder = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs("LogsFolder"));
            }
        }

        public static Dictionary<string, string> Countries
        {
            get { return _countries; }
            set
            {
                _countries = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs("CountryDict"));
            }
        }

        public static List<string> CountryList
        {
            get { return _country_list; }
            set
            {
                _country_list = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs("CountryList"));
            }
        }


        public static string CountryCode
        {
            get { return _country_code; }
            set
            {
                _country_code = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs("CurrentCountryCode"));
            }
        }

        public static string Country 
        { 
            get { return _country; } 
            set 
            { 
                _country = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs("CurrentCountry"));
            } 
        }

        public static ListCollectionView ForecastView
        {
            get { return _forecast_view; }
            set
            {
                _forecast_view = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs("CurrentForecastView"));
            }
        }

        public static ListCollectionView HistoricView
        {
            get { return _historic_view; }
            set
            {
                _historic_view = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs("CurrentForecastView"));
            }
        }

        public static ListCollectionView ForecastDbView
        {
            get { return _forecast_db_view; }
            set
            {
                _forecast_db_view = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs("CurrentForecastDbView"));
            }
        }

        public static ListCollectionView HistoricDbView
        {
            get { return _historic_db_view; }
            set
            {
                _historic_db_view = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs("CurrentHistoricDbView"));
            }
        }

        public static string ForecastJson
        {
            get { return _forecast_json; }
            set
            {
                _forecast_json = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs("CurrentForecastJson"));
            }
        }

        public static string HistoricJson
        {
            get { return _historic_json; }
            set
            {
                _historic_json = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs("CurrentHistoricJson"));
            }
        }

        public static ObservableCollection<Curve> ForecastList
        {
            get { return _forecast_list; }
            set
            {
                _forecast_list = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs("CurrentForecastList"));
            }
        }

        public static ObservableCollection<Curve> HistoricList
        {
            get { return _historic_list; }
            set
            {
                _historic_list = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs("CurrentHistoricList"));
            }
        }

        public static ObservableCollection<ForMeta> ForMetaList
        {
            get { return _for_meta_list; }
            set
            {
                _for_meta_list = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs("CurrentForMetaList"));
            }
        }

        public static ObservableCollection<HisMeta> HisMetaList
        {
            get { return _his_meta_list; }
            set
            {
                _his_meta_list = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs("CurrentHisMetaList"));
            }
        }

        public static ObservableCollection<CountryBoxItem> ListeningCountries
        {
            get { return _listening_countries; }
            set
            {
                _listening_countries = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs("CurrentListeningCountries"));
            }
        }

        public static Button AddJsonButton
        {
            get { return _add_json_button; }
            set
            {
                _add_json_button = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs("AddJsonButton"));
            }
        }

        public static Button RemoveJsonButton
        {
            get { return _remove_json_button; }
            set
            {
                _remove_json_button = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs("RemoveJsonButton"));
            }
        }

        public static Button CreateTableButton
        {
            get { return _create_table_button; }
            set
            {
                _create_table_button = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs("CreateTableButton"));
            }
        }

        public static Button DropTableButton
        {
            get { return _drop_table_button; }
            set
            {
                _drop_table_button = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs("DropTableButton"));
            }
        }

        public static JObject SecretsJson
        {
            get { return _secrets_json; }
            set
            {
                _secrets_json = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs("WsSecretsJson"));
            }
        }

        public static string SecretsPath
        {
            get { return _secrets_path; }
            set
            {
                _secrets_path = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs("WsSecretsPath"));
            }
        }

        public static HttpClient Client
        {
            get { return _client; }
            set
            {
                _client = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs("HttpClient"));
            }
        }

        public static REngine Engine
        {
            get { return _engine; }
            set
            {
                _engine = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs("HttpClient"));
            }
        }
    }

    
    public partial class WsmHome : Page
    {
        public readonly string json_pat = "*.json";
        public string[] all_array = new string[] { };
        public string[] forecast_array = new string[] { };
        public string[] historic_array = new string[] { };

        public string r_home = @"C:\Program Files\R\R-3.4.3";
        public string r_path = @"C:\Program Files\R\R-3.4.3\bin\x64";

        public WsmHome()
        {
            this.DataContext = this;
            InitializeComponent();

            SharedVariables.Countries = new Dictionary<string, string>
            {
                {"AT", "Austria"},
                {"BE", "Belgium"},
                {"CH", "Switzerland"},
                {"DE", "Germany"},
                {"DK", "Denmark"},
                {"ES", "Spain"},
                {"FR", "France"},
                {"IT", "Italy"},
                {"NL", "Netherlands"},
                {"NO", "Norway"},
                {"SE", "Sweden"},
                {"UK", "United Kingdom"}
            };

            SharedVariables.CountryList = new List<string>();
            SharedVariables.User = Environment.UserName;

            SharedVariables.SecretsPath = $@"C:\Users\{SharedVariables.User}\WATTSIGHT\JSON\SECRETS.json";
            SharedVariables.SecretsJson = JObject.Parse(File.ReadAllText(SharedVariables.SecretsPath));

            SharedVariables.LogsFolder = $@"C:\Users\{SharedVariables.User}\WATTSIGHT\WSLOGS";

            string json_path = $@"C:\Users\{SharedVariables.User}\WATTSIGHT\JSON";
            all_array = Directory.GetFiles(json_path, json_pat);

            foreach (string json_file in all_array)
            {
                string CountryCode = json_file.Substring(json_file.LastIndexOf(@"\") + 1, 2);
                string Country = SharedVariables.Countries[CountryCode];
                SharedVariables.CountryList.Add(Country);
            }

            SharedVariables.CountryList = SharedVariables.CountryList.Distinct().ToList();

            CountryListBox.DataContext = SharedVariables.CountryList;

            StartupParameter rinit = new StartupParameter
            {
                Quiet = true,
                RHome = r_home,
                Interactive = true
            };

            REngine.SetEnvironmentVariables();

            SharedVariables.Engine = REngine.GetInstance(@"C:\Program Files\R\R-3.4.3\bin\x64\R.dll", true, rinit);
            SharedVariables.Engine.Initialize();

            SharedVariables.Engine.Evaluate($@"suppressMessages(library(DT))
            suppressMessages(library(hms))
            suppressMessages(library(zoo))
            suppressMessages(library(DBI))
            suppressMessages(library(curl))
            suppressMessages(library(httr))
            suppressMessages(library(shiny))
            suppressMessages(library(readxl))
            suppressMessages(library(plotly))
            suppressMessages(library(pillar))
            suppressMessages(library(stringi))
            suppressMessages(library(RSQLite))
            suppressMessages(library(jsonlite))
            suppressMessages(library(tidyverse))
            suppressMessages(library(comprehenr))");

            string r_watt = $@"C:\Users\{SharedVariables.User}\WATTSIGHT\RLIB\rwatt.r";

            r_watt = r_watt.Replace("\\", "/");
            SharedVariables.Engine.Evaluate($@"source('{r_watt}')");
        }

        private void CountryListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            for (int i = App.Current.Windows.Count - 1; i >= 0; i--)
                if (App.Current.Windows[i].Title == "ADD JSON" || App.Current.Windows[i].Title == "REMOVE JSON" || App.Current.Windows[i].Title == "CREATE TABLE" || App.Current.Windows[i].Title == "DROP TABLE") 
                {
                    App.Current.Windows[i].Close();
                }  

            ListBox listbox = (ListBox)sender;

            int idx = listbox.SelectedIndex;
            SharedVariables.Country = listbox.SelectedItem.ToString();
            SharedVariables.CountryCode = SharedVariables.Countries.Single(x => x.Value == SharedVariables.Country).Key;
            
            SharedVariables.ForecastJson = $@"C:\Users\{SharedVariables.User}\WATTSIGHT\JSON\{SharedVariables.CountryCode}_FOR.json";
            SharedVariables.HistoricJson = $@"C:\Users\{SharedVariables.User}\WATTSIGHT\JSON\{SharedVariables.CountryCode}_HIS.json";

            string forecast_db = $@"C:\Users\{SharedVariables.User}\WATTSIGHT\SQLDB\{SharedVariables.CountryCode}_FOR.db";
            string historic_db = $@"C:\Users\{SharedVariables.User}\WATTSIGHT\SQLDB\{SharedVariables.CountryCode}_HIS.db";

            SharedVariables.ForecastList = new ObservableCollection<Curve>();
            SharedVariables.HistoricList = new ObservableCollection<Curve>();
            SharedVariables.ForMetaList = new ObservableCollection<ForMeta>();
            SharedVariables.HisMetaList = new ObservableCollection<HisMeta>();

            SharedVariables.ForecastView = new ListCollectionView(SharedVariables.ForecastList);
            forecast_data_grid.ItemsSource = SharedVariables.ForecastView;

            SharedVariables.HistoricView = new ListCollectionView(SharedVariables.HistoricList);
            historic_data_grid.ItemsSource = SharedVariables.HistoricView;

            SharedVariables.ForecastDbView = new ListCollectionView(SharedVariables.ForMetaList);
            forecast_db_grid.ItemsSource = SharedVariables.ForecastDbView;

            SharedVariables.HistoricDbView = new ListCollectionView(SharedVariables.HisMetaList);
            historic_db_grid.ItemsSource = SharedVariables.HistoricDbView;

            DateTime for_date;
            DateTime his_date;


            // JSON FORECAST

            if (File.Exists(SharedVariables.ForecastJson))
            {
                JObject forecast_catalogue = JObject.Parse(File.ReadAllText(SharedVariables.ForecastJson));

                foreach (var cluster in forecast_catalogue)
                {
                    Dictionary<string, string> for_cluster_dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(cluster.Value.ToString());

                    foreach (var for_curve in for_cluster_dict)
                    {
                        Curve for_temp_curve = new Curve() { ID = for_curve.Key, Name = for_curve.Value, Cluster = cluster.Key };
                        SharedVariables.ForecastList.Add(for_temp_curve);
                    }
                }

                SharedVariables.ForecastList.Sort(x => x.Name, true);
                SharedVariables.ForecastList = new ObservableCollection<Curve>(SharedVariables.ForecastList.OrderBy(x => x.Cluster));
                SharedVariables.ForecastView = new ListCollectionView(SharedVariables.ForecastList);
                SharedVariables.ForecastView.GroupDescriptions.Add(new PropertyGroupDescription("Cluster"));
                forecast_data_grid.ItemsSource = SharedVariables.ForecastView;
                
            }


            // JSON HISTORIC

            if (File.Exists(SharedVariables.HistoricJson))
            {
                JObject historic_catalogue = JObject.Parse(File.ReadAllText(SharedVariables.HistoricJson));

                foreach (var his_cluster in historic_catalogue)
                {
                    Dictionary<string, string> cluster_dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(his_cluster.Value.ToString());

                    foreach (var his_curve in cluster_dict)
                    {
                        Curve his_temp_curve = new Curve() { ID = his_curve.Key, Name = his_curve.Value, Cluster = his_cluster.Key };
                        SharedVariables.HistoricList.Add(his_temp_curve);
                    }
                }

                SharedVariables.HistoricList.Sort(x => x.Name, true);
                SharedVariables.HistoricList = new ObservableCollection<Curve>(SharedVariables.HistoricList.OrderBy(x => x.Cluster));
                SharedVariables.HistoricView = new ListCollectionView(SharedVariables.HistoricList);
                SharedVariables.HistoricView.GroupDescriptions.Add(new PropertyGroupDescription("Cluster"));
                historic_data_grid.ItemsSource = SharedVariables.HistoricView;
            }

            // DB FORECAST

            if (File.Exists(forecast_db))
            {
                using (SQLiteConnection for_conn = new SQLiteConnection($@"Data Source={forecast_db};"))
                {
                    for_conn.Open();

                    SQLiteCommand for_command = new SQLiteCommand("SELECT * FROM metadata ORDER BY curve_name;", for_conn);
                    SQLiteDataReader for_reader = for_command.ExecuteReader();

                    while (for_reader.Read())
                    {
                        string for_cluster = "";

                        string for_curve_name = (string)for_reader["curve_name"];

                        if (File.Exists(SharedVariables.ForecastJson))
                        {
                            try
                            {
                                for_cluster = SharedVariables.ForecastList.Single(x => x.Name == for_curve_name).Cluster;
                            }
                            catch (InvalidOperationException)
                            {
                                for_cluster = "NONE";
                            }
                        }
                        else
                        {
                            for_cluster = "NONE";
                        }

                        string for_issue_date = (string)for_reader["issue_date"];
                        if (for_issue_date.Count() > 0)
                        {
                            for_date = DateTime.Parse(for_issue_date);
                            for_issue_date = for_date.ToString("yyyy-MM-dd HH:mm");
                        }
                        
                        string for_created = (string)for_reader["created"];
                        if (for_created.Count() > 0)
                        {
                            for_date = DateTime.Parse(for_created);
                            for_created = for_date.ToString("yyyy-MM-dd HH:mm");
                        }

                        string for_modified = (string)for_reader["modified"];
                        if (for_modified.Count() > 0)
                        {
                            for_date = DateTime.Parse(for_modified);
                            for_modified = for_date.ToString("yyyy-MM-dd HH:mm");
                        }

                        string for_data_from = (string)for_reader["data_from"];
                        if (for_data_from.Count() > 0)
                        {
                            for_date = DateTime.Parse(for_data_from);
                            for_data_from = for_date.ToString("yyyy-MM-dd HH:mm");
                        }

                        string for_data_to = (string)for_reader["data_to"];
                        if (for_data_to.Count() > 0)
                        {
                            for_date = DateTime.Parse(for_data_to);
                            for_data_to = for_date.ToString("yyyy-MM-dd HH:mm");
                        }

                        ForMeta temp_for_meta = new ForMeta()
                        {
                            ForCurveId = (string)for_reader["curve_id"].ToString(),
                            ForTableName = (string)for_reader["table_name"],
                            ForCurveName = for_curve_name,
                            ForCurveType = (string)for_reader["curve_type"],
                            ForDataType = (string)for_reader["data_type"],
                            ForUnit = (string)for_reader["unit"],
                            ForFrequency = (string)for_reader["frequency"],
                            ForFunction = (string)for_reader["function"],
                            ForIssueDate = for_issue_date,
                            ForCreated = for_created,
                            ForModified = for_modified,
                            ForDataFrom = for_data_from,
                            ForDataTo = for_data_to,
                            ForDataSize = (string)for_reader["data_size"].ToString(),
                            Cluster = for_cluster
                        };

                        SharedVariables.ForMetaList.Add(temp_for_meta);
                    }

                    for_reader.Close();
                    for_conn.Close();
                }

                SharedVariables.ForMetaList.Sort(x => x.ForCurveName, true);
                SharedVariables.ForMetaList = new ObservableCollection<ForMeta>(SharedVariables.ForMetaList.OrderBy(x => x.Cluster));
                SharedVariables.ForecastDbView = new ListCollectionView(SharedVariables.ForMetaList);
                SharedVariables.ForecastDbView.GroupDescriptions.Add(new PropertyGroupDescription("Cluster"));
                forecast_db_grid.ItemsSource = SharedVariables.ForecastDbView;
            }

            if ((File.Exists(SharedVariables.ForecastJson)) && (File.Exists(forecast_db)))
            {
                List<string> json_names = SharedVariables.ForecastList.Select(x => x.Name).ToList();
                List<string> db_names = SharedVariables.ForMetaList.Select(x => x.ForCurveName).ToList();

                List<string> json_not_db = json_names.Except(db_names).ToList();
                List<string> db_not_json = db_names.Except(json_names).ToList();

                if (json_not_db.Count > 0)
                {
                    if (json_not_db.Count == 1)
                    {
                        Curve json_for_curve = SharedVariables.ForecastList.Single(x => x.Name == json_not_db[0]);
                        json_for_curve.State = "NOK";
                    }
                    else
                    {
                        foreach (string curve_name in json_not_db)
                        {
                            Curve json_for_curve = SharedVariables.ForecastList.Single(x => x.Name == curve_name);
                            json_for_curve.State = "NOK";
                        }
                    }
                }

                if (db_not_json.Count > 0)
                {
                    // Console.WriteLine(json_not_db.Count);
                    if (db_not_json.Count == 1)
                    {
                        // Console.WriteLine(db_not_json[0]);
                        ForMeta db_for_curve = SharedVariables.ForMetaList.Single(x => x.ForCurveName == db_not_json[0]);
                        db_for_curve.State = "NOK";
                    }
                    else
                    {
                        foreach (string curve_name in db_not_json)
                        {
                            ForMeta db_for_curve = SharedVariables.ForMetaList.Single(x => x.ForCurveName == curve_name);
                            db_for_curve.State = "NOK";
                        }
                    }
                }


                // DB HISTORIC

                if (File.Exists(historic_db))
                {
                    using (SQLiteConnection his_conn = new SQLiteConnection($@"Data Source={historic_db};"))
                    {
                        his_conn.Open();

                        SQLiteCommand his_command = new SQLiteCommand("SELECT * FROM metadata ORDER BY curve_name;", his_conn);
                        SQLiteDataReader his_reader = his_command.ExecuteReader();

                        while (his_reader.Read())
                        {
                            string his_cluster = "";
                            string his_curve_name = (string)his_reader["curve_name"];

                            if (File.Exists(SharedVariables.HistoricJson))
                            {
                                try
                                {
                                    his_cluster = SharedVariables.HistoricList.Single(x => x.Name == his_curve_name).Cluster;
                                }
                                catch (InvalidOperationException)
                                {
                                    his_cluster = "NONE";
                                }
                            }
                            else
                            {
                                his_cluster = "NONE";
                            }

                            string his_data_from = (string)his_reader["data_from"];
                            if (his_data_from.Count() > 0)
                            {
                                his_date = DateTime.Parse(his_data_from);
                                his_data_from = his_date.ToString("yyyy-MM-dd HH:mm");
                            }

                            string his_data_to = (string)his_reader["data_to"];
                            if (his_data_to.Count() > 0)
                            {
                                his_date = DateTime.Parse(his_data_to);
                                his_data_to = his_date.ToString("yyyy-MM-dd HH:mm");
                            }

                            HisMeta temp_his_meta = new HisMeta()
                            {
                                HisCurveId = (string)his_reader["curve_id"].ToString(),
                                HisTableName = (string)his_reader["table_name"],
                                HisCurveName = his_curve_name,
                                HisCurveType = (string)his_reader["curve_type"],
                                HisDataType = (string)his_reader["data_type"],
                                HisUnit = (string)his_reader["unit"],
                                HisFrequency = (string)his_reader["frequency"],
                                HisFunction = (string)his_reader["function"],
                                HisDataFrom = his_data_from,
                                HisDataTo = his_data_to,
                                HisDataSize = (string)his_reader["data_size"].ToString(),
                                Cluster = his_cluster
                            };

                            SharedVariables.HisMetaList.Add(temp_his_meta);
                        }

                        his_reader.Close();
                        his_conn.Close();
                    }

                    SharedVariables.HisMetaList.Sort(x => x.HisCurveName, true);
                    SharedVariables.HisMetaList = new ObservableCollection<HisMeta>(SharedVariables.HisMetaList.OrderBy(x => x.Cluster));
                    SharedVariables.HistoricDbView = new ListCollectionView(SharedVariables.HisMetaList);
                    SharedVariables.HistoricDbView.GroupDescriptions.Add(new PropertyGroupDescription("Cluster"));
                    historic_db_grid.ItemsSource = SharedVariables.HistoricDbView;
                }

                if ((File.Exists(SharedVariables.HistoricJson)) && (File.Exists(historic_db)))
                {
                    List<string> his_json_names = SharedVariables.HistoricList.Select(x => x.Name).ToList();
                    List<string> his_db_names = SharedVariables.HisMetaList.Select(x => x.HisCurveName).ToList();

                    List<string> his_json_not_db = his_json_names.Except(his_db_names).ToList();
                    List<string> his_db_not_json = his_db_names.Except(his_json_names).ToList();

                    if (his_json_not_db.Count > 0)
                    {
                        if (his_json_not_db.Count == 1)
                        {
                            Curve json_his_curve = SharedVariables.HistoricList.Single(x => x.Name == his_json_not_db[0]);
                            json_his_curve.State = "NOK";
                        }
                        else
                        {
                            foreach (string curve_name in his_json_not_db)
                            {
                                Curve json_his_curve = SharedVariables.HistoricList.Single(x => x.Name == curve_name);
                                json_his_curve.State = "NOK";
                            }
                        }
                    }

                    if (his_db_not_json.Count > 0)
                    {
                        if (his_db_not_json.Count == 1)
                        {
                            HisMeta db_his_curve = SharedVariables.HisMetaList.Single(x => x.HisCurveName == his_db_not_json[0]);
                            db_his_curve.State = "NOK";
                        }
                        else
                        {
                            foreach (string curve_name in his_db_not_json)
                            {
                                HisMeta db_his_curve = SharedVariables.HisMetaList.Single(x => x.HisCurveName == curve_name);
                                db_his_curve.State = "NOK";
                            }
                        }
                    }
                }
            }
        }

        private void Forecast_data_grid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            forecast_data_grid.UnselectAllCells();
        }

        private void Historic_data_grid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            historic_data_grid.UnselectAllCells();
        }

        private void Forecast_db_grid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            forecast_db_grid.UnselectAllCells();
        }

        private void Historic_db_grid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            historic_db_grid.UnselectAllCells();
        }

        private void OpenAddJsonForm(object sender, RoutedEventArgs e)
        {
            JsonAddForm json_add_form = new JsonAddForm();
            json_add_form.Show();
            AddJson.IsEnabled = false;
            SharedVariables.AddJsonButton = AddJson;
        }

        private void OpenRemoveJsonForm(object sender, RoutedEventArgs e)
        {
            JsonDeleteForm json_delete_form = new JsonDeleteForm();
            json_delete_form.Show();
            RemoveJson.IsEnabled = false;
            SharedVariables.RemoveJsonButton = RemoveJson;
        }

        private void AddDb_Click(object sender, RoutedEventArgs e)
        {
            DbAddForm db_add_form = new DbAddForm();
            db_add_form.Show();
            AddDb.IsEnabled = false;
            SharedVariables.CreateTableButton = AddDb;
        }

        private void RemoveDb_Click(object sender, RoutedEventArgs e)
        {
            DbDropForm db_drop_form = new DbDropForm();
            db_drop_form.Show();
            RemoveDb.IsEnabled = false;
            SharedVariables.DropTableButton = RemoveDb;
        }

        private void Dabnav_Click(object sender, RoutedEventArgs e)
        {
            SharedVariables.Engine.Evaluate(@"tz <- 'CET'; freq <- 'H'; func <- 'AVERAGE'; USER <- Sys.info()[['user']]; WORKDIR <- paste0('C:\\Users\\', USER, '\\WATTSIGHT'); setwd(WORKDIR); suppressWarnings(shiny::runApp())");
            Dabnav.IsEnabled = false;
        }

        private void Wsd_Click(object sender, RoutedEventArgs e)
        {
            Process process = new Process();

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = "powershell.exe",
                Arguments = $"/C wsd {SharedVariables.CountryCode}"
            };

            process.StartInfo = startInfo;
            process.Start();
        }

        private void Wsl_Click(object sender, RoutedEventArgs e)
        {
            Wsl.IsEnabled = false;
            NavigationService.Navigate(new Uri("WslForm.xaml", UriKind.Relative));

        }
    }
}
