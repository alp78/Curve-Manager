using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using Newtonsoft.Json;
using RDotNet;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace wsm
{
    public partial class JsonAddForm : Window
    {
        public string CountryUpper { get; set; }
        public Int32 CurveForId { get; set; }
        public string CurveForName { get; set; }
        public Int32 CurveHisId { get; set; }
        public string CurveHisName { get; set; }
        public ObservableCollection<string> ClusterList { get; set; }


        public JsonAddForm()
        {
            InitializeComponent();
            this.DataContext = this;

            CountryUpper = SharedVariables.Country.ToUpper();

            ObservableCollection<string> forecast_cluster_list = new ObservableCollection<string>();
            JObject forecast_catalogue = JObject.Parse(File.ReadAllText(SharedVariables.ForecastJson));

            foreach (var cluster in forecast_catalogue)
            {
                forecast_cluster_list.Add(cluster.Key.ToString());
            }
            ForecastClusterList.ItemsSource = forecast_cluster_list;



            ObservableCollection<string> historic_cluster_list = new ObservableCollection<string>();
            JObject historic_catalogue = JObject.Parse(File.ReadAllText(SharedVariables.HistoricJson));
            foreach (var cluster in historic_catalogue)
            {
                historic_cluster_list.Add(cluster.Key.ToString());
            }
            HistoricClusterList.ItemsSource = historic_cluster_list;

        }

        private void ForecastCheckCurve_Click(object sender, RoutedEventArgs e)
        {

            ForecastCurveId.Clear();
            ForecastCurveName.Clear();

            ForecastCurveInput.IsEnabled = false;
            ForecastCheckCurve.IsEnabled = false;
            HistoricCurveInput.IsEnabled = false;
            HistoricCheckCurve.IsEnabled = false;

            CurveForName = ForecastCurveInput.Text;
            bool found_curve = false;

            try
            {
                SharedVariables.Engine.Evaluate("ws_token <- GetToken()");

                DataFrame curve_df = SharedVariables.Engine.Evaluate($@"curve <- GetCurve(ws_token = ws_token, curve_name = '{CurveForName}')").AsDataFrame();

                for (int i = 0; i < curve_df.RowCount; ++i)
                {
                    CurveForId = Convert.ToInt32(curve_df[i, 0].ToString());
                    CurveForName = Convert.ToString(curve_df[i, 1].ToString());
                }

                byte[] bytes = Encoding.Default.GetBytes(CurveForName);
                CurveForName = Encoding.UTF8.GetString(bytes);

                found_curve = true;
            }
            catch (Exception)
            {
                MessageBox.Show("This curve doesn't exist.");
            }

            if (found_curve == true)
            {

                bool is_valid = true;
                JObject forecast_catalogue = JObject.Parse(File.ReadAllText(SharedVariables.ForecastJson));

                ForecastCurveId.Foreground = new SolidColorBrush(Colors.Green);
                ForecastCurveName.Foreground = new SolidColorBrush(Colors.Green);

                foreach (var cluster in forecast_catalogue)
                {
                    Dictionary<string, string> cluster_dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(forecast_catalogue.Value<JObject>(cluster.Key.ToString()).ToString());

                    if (cluster_dict.ContainsKey(CurveForId.ToString()))
                    {
                        ForecastCurveName.Foreground = new SolidColorBrush(Colors.Red);
                        ForecastCurveId.Foreground = new SolidColorBrush(Colors.Red);
                        ForecastClusterList.SelectedItem = cluster.Key.ToString();
                        is_valid = false;
                        break;
                    }
                }

                if (is_valid == true)
                {
                    ForecastClusterList.SelectedIndex = -1;
                    ForecastClusterList.IsEnabled = true;
                    AddJsonForecast.IsEnabled = true;
                }
                else
                {
                    ForecastClusterList.IsEnabled = false;
                    AddJsonForecast.IsEnabled = false;
                }

                ForecastCurveId.Text = $"{CurveForId}";
                ForecastCurveName.Text = $"{CurveForName}";
            }
            ForecastCurveInput.IsEnabled = true;
            ForecastCheckCurve.IsEnabled = true;
            HistoricCurveInput.IsEnabled = true;
            HistoricCheckCurve.IsEnabled = true;
        }

        private void HistoricCheckCurve_Click(object sender, RoutedEventArgs e)
        {
            HistoricCurveId.Clear();
            HistoricCurveName.Clear();

            ForecastCurveInput.IsEnabled = false;
            ForecastCheckCurve.IsEnabled = false;
            HistoricCurveInput.IsEnabled = false;
            HistoricCheckCurve.IsEnabled = false;

            CurveHisName = HistoricCurveInput.Text;
            bool found_curve = false;

            try
            {
                SharedVariables.Engine.Evaluate("ws_token <- GetToken()");

                DataFrame curve_df = SharedVariables.Engine.Evaluate($@"curve <- GetCurve(ws_token = ws_token, curve_name = '{CurveHisName}')").AsDataFrame();

                for (int i = 0; i < curve_df.RowCount; ++i)
                {
                    CurveHisId = Convert.ToInt32(curve_df[i, 0].ToString());
                    CurveHisName = Convert.ToString(curve_df[i, 1].ToString());
                }

                byte[] bytes = Encoding.Default.GetBytes(CurveHisName);
                CurveHisName = Encoding.UTF8.GetString(bytes);

                found_curve = true;
            }
            catch (Exception)
            {
                MessageBox.Show("This curve doesn't exist.");
            }

            if (found_curve == true)
            {

                bool is_valid = true;
                JObject historic_catalogue = JObject.Parse(File.ReadAllText(SharedVariables.HistoricJson));

                HistoricCurveId.Foreground = new SolidColorBrush(Colors.Green);
                HistoricCurveName.Foreground = new SolidColorBrush(Colors.Green);

                foreach (var cluster in historic_catalogue)
                {
                    Dictionary<string, string> cluster_dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(historic_catalogue.Value<JObject>(cluster.Key.ToString()).ToString());

                    if (cluster_dict.ContainsKey(CurveHisId.ToString()))
                    {
                        HistoricCurveName.Foreground = new SolidColorBrush(Colors.Red);
                        HistoricCurveId.Foreground = new SolidColorBrush(Colors.Red);
                        HistoricClusterList.SelectedItem = cluster.Key.ToString();
                        is_valid = false;

                        break;

                    }
                }

                if (is_valid == true)
                {
                    HistoricClusterList.SelectedIndex = -1;
                    HistoricClusterList.IsEnabled = true;
                    AddJsonHistoric.IsEnabled = true;
                }
                else
                {
                    HistoricClusterList.IsEnabled = false;
                    AddJsonHistoric.IsEnabled = false;
                }

                HistoricCurveId.Text = $"{CurveHisId}";
                HistoricCurveName.Text = $"{CurveHisName}";
            }
            ForecastCurveInput.IsEnabled = true;
            ForecastCheckCurve.IsEnabled = true;
            HistoricCurveInput.IsEnabled = true;
            HistoricCheckCurve.IsEnabled = true;
        }

        private void AddJsonForecast_Click(object sender, RoutedEventArgs e)
        {
            ForecastCurveInput.IsEnabled = false;
            ForecastCheckCurve.IsEnabled = false;
            HistoricCurveInput.IsEnabled = false;
            HistoricCheckCurve.IsEnabled = false;

            if (ForecastClusterList.SelectedIndex == -1)
            {
                MessageBox.Show("You must choose a cluster!");

                ForecastClusterList.SelectedIndex = -1;

            } else
            {
                string cluster = (string)ForecastClusterList.SelectedItem;

                JObject forecast_catalogue = JObject.Parse(File.ReadAllText(SharedVariables.ForecastJson));

                Dictionary<string, string> cluster_dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(forecast_catalogue.Value<JObject>(cluster).ToString());

                if (cluster_dict.ContainsKey(CurveForId.ToString()))
                {
                    ForecastCurveName.Foreground = new SolidColorBrush(Colors.Red);
                    ForecastCurveId.Foreground = new SolidColorBrush(Colors.Red);

                }
                else
                {
                    forecast_catalogue.Value<JObject>(cluster).Add(new JProperty(CurveForId.ToString(), CurveForName));

                    using (StreamWriter json_file = File.CreateText(SharedVariables.ForecastJson))
                    using (JsonTextWriter writer = new JsonTextWriter(json_file))
                    {
                        forecast_catalogue.WriteTo(writer);
                    }


                    ForecastCurveInput.Clear();
                    ForecastCurveId.Clear();
                    ForecastCurveName.Clear();
                    ForecastClusterList.SelectedIndex = -1;

                    try
                    {
                        bool curve_exists = SharedVariables.ForMetaList.Contains(SharedVariables.ForMetaList.Single(x => x.ForCurveName == CurveForName));

                        if (curve_exists == true)
                        {
                            SharedVariables.ForecastList.Add(new Curve { ID = CurveForId.ToString(), Name = CurveForName, Cluster = cluster, State = "OK" });
                            ForMeta for_meta = SharedVariables.ForMetaList.Single(x => x.ForCurveName == CurveForName);
                            for_meta.Cluster = cluster;
                            for_meta.State = "OK";

                            SharedVariables.ForMetaList.Remove(SharedVariables.ForMetaList.Where(x => x.ForCurveId == CurveForId.ToString()).Single());
                            SharedVariables.ForMetaList.Add(for_meta);
                        }
                        else
                        {
                            SharedVariables.ForecastList.Add(new Curve { ID = CurveForId.ToString(), Name = CurveForName, Cluster = cluster, State = "NOK" });
                        }
                    }
                    catch (InvalidOperationException)
                    {
                        SharedVariables.ForecastList.Add(new Curve { ID = CurveForId.ToString(), Name = CurveForName, Cluster = cluster, State = "NOK" });
                    }
                }
                SharedVariables.ForMetaList.Sort(x => x.ForCurveName, true);
                SharedVariables.ForecastList.Sort(x => x.Name, true);
            }
            ForecastCurveInput.IsEnabled = true;
            ForecastCheckCurve.IsEnabled = true;
            HistoricCurveInput.IsEnabled = true;
            HistoricCheckCurve.IsEnabled = true;
        }

        private void AddJsonHistoric_Click(object sender, RoutedEventArgs e)
        {
            ForecastCurveInput.IsEnabled = false;
            ForecastCheckCurve.IsEnabled = false;
            HistoricCurveInput.IsEnabled = false;
            HistoricCheckCurve.IsEnabled = false;

            if (HistoricClusterList.SelectedIndex == -1)
            {
                MessageBox.Show("You must choose a cluster!");

                HistoricClusterList.SelectedIndex = -1;
            }
            else
            {
                string cluster = (string)HistoricClusterList.SelectedItem;

                JObject historic_catalogue = JObject.Parse(File.ReadAllText(SharedVariables.HistoricJson));

                Dictionary<string, string> cluster_dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(historic_catalogue.Value<JObject>(cluster).ToString());

                if (cluster_dict.ContainsKey(CurveHisId.ToString()))
                {
                    HistoricCurveName.Foreground = new SolidColorBrush(Colors.Red);
                    HistoricCurveId.Foreground = new SolidColorBrush(Colors.Red);

                }
                else
                {
                    historic_catalogue.Value<JObject>(cluster).Add(new JProperty(CurveHisId.ToString(), CurveHisName));

                    using (StreamWriter json_file = File.CreateText(SharedVariables.HistoricJson))
                    using (JsonTextWriter writer = new JsonTextWriter(json_file))
                    {
                        historic_catalogue.WriteTo(writer);
                    }

                    HistoricCurveInput.Clear();
                    HistoricCurveId.Clear();
                    HistoricCurveName.Clear();
                    HistoricClusterList.SelectedIndex = -1;

                    try
                    {
                        bool curve_exists = SharedVariables.HisMetaList.Contains(SharedVariables.HisMetaList.Single(x => x.HisCurveName == CurveHisName));

                        if (curve_exists == true)
                        {
                            SharedVariables.HistoricList.Add(new Curve { ID = CurveHisId.ToString(), Name = CurveHisName, Cluster = cluster, State = "OK" });
                            HisMeta his_meta = SharedVariables.HisMetaList.Single(x => x.HisCurveName == CurveHisName);
                            his_meta.Cluster = cluster;
                            his_meta.State = "OK";

                            SharedVariables.HisMetaList.Remove(SharedVariables.HisMetaList.Where(x => x.HisCurveId == CurveHisId.ToString()).Single());
                            SharedVariables.HisMetaList.Add(his_meta);
                        }
                        else
                        {
                            SharedVariables.HistoricList.Add(new Curve { ID = CurveHisId.ToString(), Name = CurveHisName, Cluster = cluster, State = "NOK" });
                        }
                    }
                    catch (InvalidOperationException)
                    {
                        SharedVariables.HistoricList.Add(new Curve { ID = CurveHisId.ToString(), Name = CurveHisName, Cluster = cluster, State = "NOK" });
                    }
                }
                SharedVariables.HisMetaList.Sort(x => x.HisCurveName, true);
                SharedVariables.HistoricList.Sort(x => x.Name, true);
            }
            ForecastCurveInput.IsEnabled = true;
            ForecastCheckCurve.IsEnabled = true;
            HistoricCurveInput.IsEnabled = true;
            HistoricCheckCurve.IsEnabled = true;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SharedVariables.AddJsonButton.IsEnabled = true;
        }
    }
}
