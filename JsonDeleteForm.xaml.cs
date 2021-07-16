using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace wsm
{
    public partial class JsonDeleteForm : Window
    {
        public string CountryUpper { get; set; }
        public JsonDeleteForm()
        {
            InitializeComponent();

            this.DataContext = this;

            JsonForecast.ItemsSource = SharedVariables.ForecastView;
            JsonHistoric.ItemsSource = SharedVariables.HistoricView;

            CountryUpper = SharedVariables.Country.ToUpper();
        }

        private void JsonHistoric_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            JsonHistoric.UnselectAllCells();
        }

        private void JsonForecast_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            JsonForecast.UnselectAllCells();
        }

        private void RemoveJsonForecast_Click(object sender, RoutedEventArgs e)
        {
            Curve forecast_curve = (Curve)JsonForecast.SelectedItem;

            JObject forecast_catalogue = JObject.Parse(File.ReadAllText(SharedVariables.ForecastJson));

            forecast_catalogue.Value<JObject>(forecast_curve.Cluster).Remove(forecast_curve.ID);

            using (StreamWriter json_file = File.CreateText(SharedVariables.ForecastJson))
            using (JsonTextWriter writer = new JsonTextWriter(json_file))
            {
                forecast_catalogue.WriteTo(writer);
            }

            SharedVariables.ForecastList.Remove(SharedVariables.ForecastList.Where(x => x.ID == forecast_curve.ID).Single());
            JsonForecast.ItemsSource = SharedVariables.ForecastView;

            try
            {
                bool curve_exists = SharedVariables.ForMetaList.Contains(SharedVariables.ForMetaList.Single(x => x.ForCurveName == forecast_curve.Name));

                if (curve_exists == true)
                {
                    ForMeta for_meta = SharedVariables.ForMetaList.Single(x => x.ForCurveName == forecast_curve.Name);
                    for_meta.Cluster = "NONE";
                    for_meta.State = "NOK";

                    SharedVariables.ForMetaList.Remove(SharedVariables.ForMetaList.Where(x => x.ForCurveId == forecast_curve.ID.ToString()).Single());
                    SharedVariables.ForMetaList.Add(for_meta);
                }
            }
            catch (InvalidOperationException)
            {

                
            }


        }

        private void RemoveJsonHistoric_Click(object sender, RoutedEventArgs e)
        {
            Curve historic_curve = (Curve)JsonHistoric.SelectedItem;

            JObject historic_catalogue = JObject.Parse(File.ReadAllText(SharedVariables.HistoricJson));

            historic_catalogue.Value<JObject>(historic_curve.Cluster).Remove(historic_curve.ID);

            using (StreamWriter json_file = File.CreateText(SharedVariables.HistoricJson))
            using (JsonTextWriter writer = new JsonTextWriter(json_file))
            {
                historic_catalogue.WriteTo(writer);
            }

            SharedVariables.HistoricList.Remove(SharedVariables.HistoricList.Where(x => x.ID == historic_curve.ID).Single());

            JsonHistoric.ItemsSource = SharedVariables.HistoricView;

            try
            {
                bool curve_exists = SharedVariables.HisMetaList.Contains(SharedVariables.HisMetaList.Single(x => x.HisCurveName == historic_curve.Name));

                if (curve_exists == true)
                {
                    HisMeta his_meta = SharedVariables.HisMetaList.Single(x => x.HisCurveName == historic_curve.Name);
                    his_meta.Cluster = "NONE";
                    his_meta.State = "NOK";

                    SharedVariables.HisMetaList.Remove(SharedVariables.HisMetaList.Where(x => x.HisCurveId == historic_curve.ID.ToString()).Single());
                    SharedVariables.HisMetaList.Add(his_meta);
                }
            }
            catch (InvalidOperationException)
            {

            }

        }

        private void JsonDeleteFormWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SharedVariables.RemoveJsonButton.IsEnabled = true;
        }
    }
}
