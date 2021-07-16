using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;


namespace wsm
{
    
    public partial class DbDropForm : Window
    {
        public string CountryCodeUpper { get; set; }
        public string CountryUpper { get; set; }
        public string CurveForName { get; set; }
        public string CurveHisName { get; set; }
        public string ForDb { get; set; }
        public string HisDb { get; set; }

        public DbDropForm()
        {
            InitializeComponent();

            this.DataContext = this;

            DbForecast.ItemsSource = SharedVariables.ForecastDbView;
            DbHistoric.ItemsSource = SharedVariables.HistoricDbView;

            CountryUpper = SharedVariables.Country.ToUpper();
        }

        private void DbForecast_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DbForecast.UnselectAllCells();
        }

        private void DbHistoric_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DbHistoric.UnselectAllCells();
        }

        private void RemoveDbForecast_Click(object sender, RoutedEventArgs e)
        {
            ForMeta forecast_curve = (ForMeta)DbForecast.SelectedItem;
            CurveForName = forecast_curve.ForCurveName;

            CountryUpper = SharedVariables.Country.ToUpper();
            CountryCodeUpper = SharedVariables.CountryCode.ToUpper();
            ForDb = $@"C:/Users/{SharedVariables.User}/WATTSIGHT/SQLDB/{CountryCodeUpper}_FOR.db";

            SharedVariables.Engine.Evaluate($@"SqlDropCurveTable(db_path = '{ForDb}', curve_name = '{CurveForName}')");

            SharedVariables.ForMetaList.Remove(SharedVariables.ForMetaList.Where(x => x.ForCurveName == CurveForName).Single());
            DbForecast.ItemsSource = SharedVariables.ForecastDbView;

            try
            {
                bool curve_exists = SharedVariables.ForecastList.Contains(SharedVariables.ForecastList.Single(x => x.Name == CurveForName));

                if (curve_exists == true)
                {
                    Curve for_json = SharedVariables.ForecastList.Single(x => x.Name == CurveForName);
                    for_json.State = "NOK";

                    SharedVariables.ForecastList.Remove(SharedVariables.ForecastList.Where(x => x.Name == CurveForName).Single());
                    SharedVariables.ForecastList.Add(for_json);
                    SharedVariables.ForecastList.Sort(x => x.Name, true);
                }
            }
            catch (InvalidOperationException)
            {

            }
        }

        private void RemoveDbHistoric_Click(object sender, RoutedEventArgs e)
        {
            HisMeta historic_curve = (HisMeta)DbHistoric.SelectedItem;
            CurveHisName = historic_curve.HisCurveName;

            CountryCodeUpper = SharedVariables.CountryCode.ToUpper();
            HisDb = $@"C:/Users/{SharedVariables.User}/WATTSIGHT/SQLDB/{CountryCodeUpper}_HIS.db";

            SharedVariables.Engine.Evaluate($@"SqlDropCurveTable(db_path = '{HisDb}', curve_name = '{CurveHisName}')");

            SharedVariables.HisMetaList.Remove(SharedVariables.HisMetaList.Where(x => x.HisCurveName == CurveHisName).Single());
            DbHistoric.ItemsSource = SharedVariables.HistoricDbView;

            try
            {
                bool curve_exists = SharedVariables.HistoricList.Contains(SharedVariables.HistoricList.Single(x => x.Name == CurveHisName));

                if (curve_exists == true)
                {
                    Curve his_json = SharedVariables.HistoricList.Single(x => x.Name == CurveHisName);
                    his_json.State = "NOK";

                    SharedVariables.HistoricList.Remove(SharedVariables.HistoricList.Where(x => x.Name == CurveHisName).Single());
                    SharedVariables.HistoricList.Add(his_json);
                    SharedVariables.HistoricList.Sort(x => x.Name, true);
                }
            }
            catch (InvalidOperationException)
            {


            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SharedVariables.DropTableButton.IsEnabled = true;
        }
    }
}
