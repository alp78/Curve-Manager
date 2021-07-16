using RDotNet;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace wsm
{
    /// <summary>
    /// Interaction logic for DbAddForm.xaml
    /// </summary>
    public partial class DbAddForm : Window
    {
        public string CountryCodeUpper { get; set; }
        public string CountryUpper { get; set; }
        public Int32 CurveForId { get; set; }
        public Int32 CurveHisId { get; set; }
        public string CurveForName { get; set; }
        public string CurveHisName { get; set; }
        public string ForDb { get; set; }
        public string HisDb { get; set; }

        public DateTime temp_dt;

        public string temp_st;
        public DataFrame ForCurveDf { get; set; }

        public DataFrame HisCurveDf { get; set; }

        byte[] Bytes { get; set; }

        public string[] charsToRemove = new string[] { "/", "-", ">" };

        public DbAddForm()
        {
            InitializeComponent();
            this.DataContext = this;

            CountryUpper = SharedVariables.Country.ToUpper();
        }

        private void ForecastCheckCurve_Click(object sender, RoutedEventArgs e)
        {
            ForecastCheckCurve.IsEnabled = false;
            ForecastCurveInput.IsEnabled = false;

            ForCurveIdBox.Clear();
            ForCurveNameBox.Clear();
            ForCurveTypeBox.Clear();
            ForDataTypeBox.Clear();
            ForUnitBox.Clear();
            ForFrequencyBox.Clear();
            ForTimeZoneBox.Clear();
            ForCreatedBox.Clear();
            ForModifiedBox.Clear();
            TableForTableName.Clear();
            TableForDataFrom.Clear();
            TableForDataTo.Clear();
            TableForDataSize.Clear();
            TableForIssueDate.Clear();

            CurveForName = ForecastCurveInput.Text;

            bool found_curve = false;
            
            try
            {
                SharedVariables.Engine.Evaluate("ws_token <- GetToken()");

                ForCurveDf = SharedVariables.Engine.Evaluate($@"curve <- GetCurve(ws_token = ws_token, curve_name = '{CurveForName}')").AsDataFrame();

                found_curve = true;
            }
            catch (Exception)
            {
                MessageBox.Show("This curve doesn't exist.");
            }

            if (found_curve == true)
            {
                ForCurveIdBox.Foreground = new SolidColorBrush(Colors.Green);
                ForCurveNameBox.Foreground = new SolidColorBrush(Colors.Green);
                ForCurveTypeBox.Foreground = new SolidColorBrush(Colors.Green);
                ForDataTypeBox.Foreground = new SolidColorBrush(Colors.Green);
                ForUnitBox.Foreground = new SolidColorBrush(Colors.Green);
                ForFrequencyBox.Foreground = new SolidColorBrush(Colors.Green);
                ForTimeZoneBox.Foreground = new SolidColorBrush(Colors.Green);
                ForCreatedBox.Foreground = new SolidColorBrush(Colors.Green);
                ForModifiedBox.Foreground = new SolidColorBrush(Colors.Green);

                for (int i = 0; i < ForCurveDf.RowCount; ++i)
                {
                    ForCurveIdBox.Text = ForCurveDf[i, 0].ToString();
                    ForCurveNameBox.Text = CurveForName;
                    ForCurveTypeBox.Text = ForCurveDf[i, 2].ToString();
                    ForDataTypeBox.Text = ForCurveDf[i, 3].ToString();

                    Bytes = Encoding.Default.GetBytes(ForCurveDf[i, 4].ToString());
                    ForUnitBox.Text = Encoding.UTF8.GetString(Bytes);

                    ForFrequencyBox.Text = ForCurveDf[i, 5].ToString();
                    ForTimeZoneBox.Text = ForCurveDf[i, 6].ToString();

                    DateTime.TryParseExact(ForCurveDf[i, 7].ToString(), "yyyy-MM-ddTHH:mm:ss.fffzzz", CultureInfo.InvariantCulture, DateTimeStyles.None, out temp_dt);
                    temp_st = temp_dt.ToString("yyyy-MM-dd HH:mm");
                    ForCreatedBox.Text = temp_st;

                    DateTime.TryParseExact(ForCurveDf[i, 8].ToString(), "yyyy-MM-ddTHH:mm:ss.fffzzz", CultureInfo.InvariantCulture, DateTimeStyles.None, out temp_dt);
                    temp_st = temp_dt.ToString("yyyy-MM-dd HH:mm");
                    ForModifiedBox.Text = temp_st;

                    CurveForId = Convert.ToInt32(ForCurveDf[i, 0].ToString());
                }

                List<string> forname_list = new List<string>();

                bool is_valid = true;
                foreach (ForMeta curve in SharedVariables.ForMetaList)
                {
                    forname_list.Add(curve.ForCurveName);
                }

                if (forname_list.Contains(CurveForName))
                {
                    ForCurveIdBox.Foreground = new SolidColorBrush(Colors.Red);
                    ForCurveNameBox.Foreground = new SolidColorBrush(Colors.Red);
                    ForCurveTypeBox.Foreground = new SolidColorBrush(Colors.Red);
                    ForDataTypeBox.Foreground = new SolidColorBrush(Colors.Red);
                    ForUnitBox.Foreground = new SolidColorBrush(Colors.Red);
                    ForFrequencyBox.Foreground = new SolidColorBrush(Colors.Red);
                    ForTimeZoneBox.Foreground = new SolidColorBrush(Colors.Red);
                    ForCreatedBox.Foreground = new SolidColorBrush(Colors.Red);
                    ForModifiedBox.Foreground = new SolidColorBrush(Colors.Red);

                    is_valid = false;
                }

                if (is_valid == true)
                {
                    ForCreateTable.IsEnabled = true;
                }
                else
                {
                    ForCreateTable.IsEnabled = false;
                }
            }
            ForecastCheckCurve.IsEnabled = true;
            ForecastCurveInput.IsEnabled = true;
        }


        private void HistoricCheckCurve_Click(object sender, RoutedEventArgs e)
        {
            HisCurveIdBox.Clear();
            HisCurveNameBox.Clear();
            HisCurveTypeBox.Clear();
            HisDataTypeBox.Clear();
            HisUnitBox.Clear();
            HisFrequencyBox.Clear();
            HisTimeZoneBox.Clear();
            HisCreatedBox.Clear();
            HisModifiedBox.Clear();
            TableHisTableName.Clear();
            TableHisDataFrom.Clear();
            TableHisDataTo.Clear();
            TableHisDataSize.Clear();


            CurveHisName = HistoricCurveInput.Text;

            bool found_curve = false;

            try
            {
                SharedVariables.Engine.Evaluate("ws_token <- GetToken()");

                HisCurveDf = SharedVariables.Engine.Evaluate($@"curve <- GetCurve(ws_token = ws_token, curve_name = '{CurveHisName}')").AsDataFrame();

                found_curve = true;
            }
            catch (Exception)
            {
                MessageBox.Show("This curve doesn't exist.");
            }

            if (found_curve == true)
            {

                HisCurveIdBox.Foreground = new SolidColorBrush(Colors.Green);
                HisCurveNameBox.Foreground = new SolidColorBrush(Colors.Green);
                HisCurveTypeBox.Foreground = new SolidColorBrush(Colors.Green);
                HisDataTypeBox.Foreground = new SolidColorBrush(Colors.Green);
                HisUnitBox.Foreground = new SolidColorBrush(Colors.Green);
                HisFrequencyBox.Foreground = new SolidColorBrush(Colors.Green);
                HisTimeZoneBox.Foreground = new SolidColorBrush(Colors.Green);
                HisCreatedBox.Foreground = new SolidColorBrush(Colors.Green);
                HisModifiedBox.Foreground = new SolidColorBrush(Colors.Green);

                string table_name = CurveHisName;
                table_name = table_name.Replace(' ', '_');

                foreach (string c in charsToRemove)
                {
                    table_name = table_name.Replace(c, string.Empty);
                }

                for (int i = 0; i < HisCurveDf.RowCount; ++i)
                {
                    HisCurveIdBox.Text = HisCurveDf[i, 0].ToString();
                    HisCurveNameBox.Text = CurveHisName;
                    HisCurveTypeBox.Text = HisCurveDf[i, 2].ToString();
                    HisDataTypeBox.Text = HisCurveDf[i, 3].ToString();

                    Bytes = Encoding.Default.GetBytes(HisCurveDf[i, 4].ToString());
                    HisUnitBox.Text = Encoding.UTF8.GetString(Bytes);

                    HisFrequencyBox.Text = HisCurveDf[i, 5].ToString();
                    HisTimeZoneBox.Text = HisCurveDf[i, 6].ToString();

                    DateTime.TryParseExact(HisCurveDf[i, 7].ToString(), "yyyy-MM-ddTHH:mm:ss.fffzzz", CultureInfo.InvariantCulture, DateTimeStyles.None, out temp_dt);
                    temp_st = temp_dt.ToString("yyyy-MM-dd HH:mm");
                    HisCreatedBox.Text = temp_st;

                    DateTime.TryParseExact(HisCurveDf[i, 8].ToString(), "yyyy-MM-ddTHH:mm:ss.fffzzz", CultureInfo.InvariantCulture, DateTimeStyles.None, out temp_dt);
                    temp_st = temp_dt.ToString("yyyy-MM-dd HH:mm");
                    HisModifiedBox.Text = temp_st;

                    CurveHisId = Convert.ToInt32(HisCurveDf[i, 0].ToString());
                }

                List<string> hisname_list = new List<string>();

                bool is_valid = true;
                foreach (HisMeta curve in SharedVariables.HisMetaList)
                {
                    hisname_list.Add(curve.HisCurveName);
                }

                if (hisname_list.Contains(CurveHisName))
                {
                    HisCurveIdBox.Foreground = new SolidColorBrush(Colors.Red);
                    HisCurveNameBox.Foreground = new SolidColorBrush(Colors.Red);
                    HisCurveTypeBox.Foreground = new SolidColorBrush(Colors.Red);
                    HisDataTypeBox.Foreground = new SolidColorBrush(Colors.Red);
                    HisUnitBox.Foreground = new SolidColorBrush(Colors.Red);
                    HisFrequencyBox.Foreground = new SolidColorBrush(Colors.Red);
                    HisTimeZoneBox.Foreground = new SolidColorBrush(Colors.Red);
                    HisCreatedBox.Foreground = new SolidColorBrush(Colors.Red);
                    HisModifiedBox.Foreground = new SolidColorBrush(Colors.Red);

                    is_valid = false;
                }

                if (is_valid == true)
                {
                    HisCreateTable.IsEnabled = true;
                }
                else
                {
                    HisCreateTable.IsEnabled = false;
                }

            }
        }

        private void ForCreateTable_Click(object sender, RoutedEventArgs e)
        {
            ForBar.Visibility = Visibility.Visible;
            ForCreateTable.IsEnabled = false;
            ForecastCheckCurve.IsEnabled = false;
            HistoricCheckCurve.IsEnabled = false;
            ForecastCurveInput.IsEnabled = false;
            HistoricCurveInput.IsEnabled = false;

            CountryCodeUpper = SharedVariables.CountryCode.ToUpper();
            ForDb = $@"C:/Users/{SharedVariables.User}/WATTSIGHT/SQLDB/{CountryCodeUpper}_FOR.db";

            SharedVariables.Engine.Evaluate("ws_token <- GetToken()");
            SharedVariables.Engine.Evaluate($@"SqlCreateForecastTable(ws_token = ws_token, db_path = '{ForDb}', curve_name = '{CurveForName}', freq = 'H', func = 'AVERAGE', tz = 'CET')").AsDataFrame();

            string table_name = CurveForName;
            table_name = table_name.Replace(' ', '_');

            foreach (string c in charsToRemove)
            {
                table_name = table_name.Replace(c, string.Empty);
            }

            TableForTableName.Text = table_name;

            if (File.Exists(ForDb))
            {
                using (SQLiteConnection for_conn = new SQLiteConnection($@"Data Source={ForDb};"))
                {
                    for_conn.Open();

                    SQLiteCommand for_command = new SQLiteCommand($"SELECT * FROM metadata WHERE curve_name = '{CurveForName}'", for_conn);
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
                                Curve for_curve = SharedVariables.ForecastList.Single(x => x.Name == for_curve_name);
                                for_curve.State = "OK";
                                SharedVariables.ForecastList.Remove(SharedVariables.ForecastList.Where(x => x.Name == CurveForName).Single());
                                SharedVariables.ForecastList.Add(for_curve);
                                
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
                            temp_dt = DateTime.Parse(for_issue_date);
                            for_issue_date = temp_dt.ToString("yyyy-MM-dd HH:mm");
                        }
                        TableForIssueDate.Text = for_issue_date;

                        string for_created = (string)for_reader["created"];
                        if (for_created.Count() > 0)
                        {
                            temp_dt = DateTime.Parse(for_created);
                            for_created = temp_dt.ToString("yyyy-MM-dd HH:mm");
                        }

                        string for_modified = (string)for_reader["modified"];
                        if (for_modified.Count() > 0)
                        {
                            temp_dt = DateTime.Parse(for_modified);
                            for_modified = temp_dt.ToString("yyyy-MM-dd HH:mm");
                        }

                        string for_data_from = (string)for_reader["data_from"];
                        if (for_data_from.Count() > 0)
                        {
                            temp_dt = DateTime.Parse(for_data_from);
                            for_data_from = temp_dt.ToString("yyyy-MM-dd HH:mm");
                        }
                        TableForDataFrom.Text = for_data_from;

                        string for_data_to = (string)for_reader["data_to"];
                        if (for_data_to.Count() > 0)
                        {
                            temp_dt = DateTime.Parse(for_data_to);
                            for_data_to = temp_dt.ToString("yyyy-MM-dd HH:mm");
                        }
                        
                        TableForDataTo.Text = for_data_to;

                        TableForDataSize.Text = (string)for_reader["data_size"].ToString();

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
                            Cluster = for_cluster,
                            State = "OK"
                        };

                        SharedVariables.ForMetaList.Add(temp_for_meta);

                        SharedVariables.ForMetaList.Sort(x => x.ForCurveName, true);
                        SharedVariables.ForecastList.Sort(x => x.Name, true);
                    }

                    for_reader.Close();
                }

                ForCurveIdBox.Clear();
                ForCurveNameBox.Clear();
                ForCurveTypeBox.Clear();
                ForDataTypeBox.Clear();
                ForUnitBox.Clear();
                ForFrequencyBox.Clear();
                ForTimeZoneBox.Clear();
                ForCreatedBox.Clear();
                ForModifiedBox.Clear();
            }

            ForBar.Visibility = Visibility.Hidden;
            ForecastCheckCurve.IsEnabled = true;
            HistoricCheckCurve.IsEnabled = true;
            ForecastCurveInput.IsEnabled = true;
            HistoricCurveInput.IsEnabled = true;
        }

        private void HisCreateTable_Click(object sender, RoutedEventArgs e)
        {
            HisBar.Visibility = Visibility.Visible;
            HisCreateTable.IsEnabled = false;
            ForecastCheckCurve.IsEnabled = false;
            HistoricCheckCurve.IsEnabled = false;
            ForecastCurveInput.IsEnabled = false;
            HistoricCurveInput.IsEnabled = false;

            CountryCodeUpper = SharedVariables.CountryCode.ToUpper();
            HisDb = $@"C:/Users/{SharedVariables.User}/WATTSIGHT/SQLDB/{CountryCodeUpper}_HIS.db";

            SharedVariables.Engine.Evaluate($@"SqlCreateHistoryTable(db_path = '{HisDb}', offset= 24, duration = 168, curve_name = '{CurveHisName}', data_from = '2016-10-01', freq = 'H', func = 'AVERAGE', tz = 'CET')").AsDataFrame();

            string table_name = CurveHisName;
            table_name = table_name.Replace(' ', '_');

            foreach (string c in charsToRemove)
            {
                table_name = table_name.Replace(c, string.Empty);
            }

            TableHisTableName.Text = table_name;

            if (File.Exists(HisDb))
            {
                using (SQLiteConnection his_conn = new SQLiteConnection($@"Data Source={HisDb};"))
                {
                    his_conn.Open();

                    SQLiteCommand his_command = new SQLiteCommand($"SELECT * FROM metadata WHERE curve_name = '{CurveHisName}'", his_conn);
                    SQLiteDataReader his_reader = his_command.ExecuteReader();

                    while (his_reader.Read())
                    {
                        string his_cluster = "";

                        string his_curve_name = (string)his_reader["curve_name"];

                        if (File.Exists(SharedVariables.HistoricJson))
                        {
                            try
                            {
                                his_cluster = SharedVariables.HistoricList.Single(x => x.Name == CurveHisName).Cluster;
                                Curve his_curve = SharedVariables.HistoricList.Single(x => x.Name == CurveHisName);
                                his_curve.State = "OK";
                                SharedVariables.HistoricList.Remove(SharedVariables.HistoricList.Where(x => x.Name == CurveHisName).Single());
                                SharedVariables.HistoricList.Add(his_curve);
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
                            temp_dt = DateTime.Parse(his_data_from);
                            his_data_from = temp_dt.ToString("yyyy-MM-dd HH:mm");
                        }
                        TableHisDataFrom.Text = his_data_from;

                        string his_data_to = (string)his_reader["data_to"];
                        if (his_data_to.Count() > 0)
                        {
                            temp_dt = DateTime.Parse(his_data_to);
                            his_data_to = temp_dt.ToString("yyyy-MM-dd HH:mm");
                        }

                        TableHisDataTo.Text = his_data_to;

                        TableHisDataSize.Text = (string)his_reader["data_size"].ToString();

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
                            Cluster = his_cluster,
                            State = "OK"
                        };

                        SharedVariables.HisMetaList.Add(temp_his_meta);

                        SharedVariables.HisMetaList.Sort(x => x.HisCurveName, true);
                        SharedVariables.HistoricList.Sort(x => x.Name, true);
                    }

                    his_reader.Close();
                }

                HisCurveIdBox.Clear();
                HisCurveNameBox.Clear();
                HisCurveTypeBox.Clear();
                HisDataTypeBox.Clear();
                HisUnitBox.Clear();
                HisFrequencyBox.Clear();
                HisTimeZoneBox.Clear();
                HisCreatedBox.Clear();
                HisModifiedBox.Clear();
            }

            HisBar.Visibility = Visibility.Hidden;
            ForecastCheckCurve.IsEnabled = true;
            HistoricCheckCurve.IsEnabled = true;
            ForecastCurveInput.IsEnabled = true;
            HistoricCurveInput.IsEnabled = true;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SharedVariables.CreateTableButton.IsEnabled = true;
        }
    }
}
