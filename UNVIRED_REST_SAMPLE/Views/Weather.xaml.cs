using Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Unvired.Kernel.Sync;
using Unvired.Kernel.Utils;
using Unvired.Kernel.UWP.Constants;
using Unvired.Kernel.UWP.Log;
using Unvired.Kernel.UWP.Model;
using Unvired.Kernel.UWP.Sync;
using UNVIRED_REST_SAMPLE.Utility;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UNVIRED_REST_SAMPLE
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Weather : Page
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        private WEATHER_HEADER _weatherHeaderInput;
        public WEATHER_HEADER WeatherHeaderInput
        {
            get => _weatherHeaderInput;
            set
            {
                _weatherHeaderInput = value;
                RaisePropertyChanged("WeatherHeaderInput");
            }
        }
        private WEATHER_HEADER _weatherHeaderResponse;
        public WEATHER_HEADER WeatherHeaderResponse
        {
            get => _weatherHeaderResponse;
            set
            {
                _weatherHeaderResponse = value;
                RaisePropertyChanged("WeatherHeaderResponse");
            }
        }

        public Weather()
        {
            this.InitializeComponent();
            if (WeatherHeaderInput == null) WeatherHeaderInput = new WEATHER_HEADER();
            WeatherHeaderResponse = new WEATHER_HEADER();
            ValidationTextBlock.Visibility = Visibility.Collapsed;
            displayGrid.Visibility = Visibility.Collapsed;
        }

        private async void GetWeatherClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(WeatherHeaderInput.CITY)) { ValidationTextBlock.Visibility = Visibility.Visible; return; }
            if (!ConnectionHelper.HasAnyInternetConnection())
            {
                var connectionDialog = Util.AlertContentDialog("No Internet", "No Internet ConnectionPlease Try Again Later", "OK");
                await connectionDialog.ShowAsync();
                return;
            }
            await CallPAForWeather(WeatherHeaderInput);
        }

        private void CityName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ValidationTextBlock.Visibility == Visibility.Visible)
                ValidationTextBlock.Visibility = Visibility.Collapsed;
            displayGrid.Visibility = Visibility.Collapsed;

        }

        private async Task CallPAForWeather(WEATHER_HEADER weatherHeaderInput)
        {
            Util.ShowProgressDialog("Please wait getting the weather");
            try
            {
                if (!string.IsNullOrEmpty(weatherHeaderInput.CITY))
                {
                    var iSyncResponse = await SyncEngine.Instance.SubmitInForeground(MessageRequestType.RQST,
                  weatherHeaderInput, null, AppConstants.PA_GET_WEATHER, false);
                    var response = (SyncBEResponse)iSyncResponse;
                    var infoMessages = response.InfoMessages;

                    if (infoMessages != null && infoMessages.Any())
                    {
                        foreach (var infoMessage in infoMessages)
                        {
                            if (!infoMessage.Category.Equals((ISyncResponse.RESPONSE_STATUS.FAILURE).ToString())) continue;
                            Util.HideProgressDialog();
                            var result = Util.FailureAlert("Error", "Failed get SAP Respose", "OK");
                            await result.ShowAsync();
                            Logger.E($"Error occur while receiving the response {infoMessage.Message}");
                            return;
                        }
                    }

                    if (response.ResponseStatus == ISyncResponse.RESPONSE_STATUS.SUCCESS)
                    {
                        BindResponseFromUMP(response.DataBEs);
                        displayGrid.Visibility = Visibility.Visible;

                    }
                }
                else
                {
                    await Util.InformationAlert("Alert..!", $"Please enter City Name", "OK").ShowAsync();
                }
            }
            catch (Exception e)
            {
                Logger.E($"Exception caught while getting document {e.Message}");
            }
            Util.HideProgressDialog();
        }
        private void BindResponseFromUMP(Dictionary<string, Dictionary<DataStructure, List<DataStructure>>> dataBEs)
        {
            try
            {


                for (int i = 0; i < dataBEs.Count; i++)
                {
                    var item = dataBEs.ElementAt(i);
                    if (item.Key.Equals("WEATHER"))
                    {
                        for (int j = 0; j < item.Value.Count; j++)
                        {
                            var headerData = (item.Value).ElementAt(j);

                            WeatherHeaderResponse = (WEATHER_HEADER)headerData.Key;
                            description.Text = WeatherHeaderResponse.WEATHER_DESC;
                            temperature.Text = WeatherHeaderResponse.TEMPERATURE;
                            humidity.Text = WeatherHeaderResponse.HUMIDITY;

                        }
                    }
                }


            }
            catch (Exception e)
            {
                Logger.E($"Exception caught while binding data to the physical inventory detail page {e.Message}");
            }
        }

        private void SettingButton_Click(object sender, RoutedEventArgs e)
        {
            SettingSplitView.IsPaneOpen = true;
        }

        private void EmailBtn_Click(object sender, RoutedEventArgs e)
        {
            Logger.SendLogViaEmail();
        }

        private void SendLogToServerBtn_Click(object sender, RoutedEventArgs e)
        {
            Logger.SendLogToServer();
        }

        private void ViewLogBtn_Click(object sender, RoutedEventArgs e)
        {
            Logger.GetLogViewer();
        }

        private async void ClearLogsBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var clearLogConfirmationDialog = Util.CommonContentDialog(Util.GetString("Alert"), "This will clear all the Logs associated with this application. Do you want to clear log", "Yes", "Cancel");
                var clearLogResult = await clearLogConfirmationDialog.ShowAsync();

                if (clearLogResult == ContentDialogResult.Primary)
                {
                    Logger.DeleteLogs();
                }
            }
            catch (Exception ex)
            {
                Logger.E($"Exception caught while clearing the application logs. Message {ex.Message}");
            }
        }

        private async void ClearDataBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var clearDataConfirmationDialog = Util.CommonContentDialog(Util.GetString("Clear Application Data"), "This will clear all application related data. Are you sure you want to continue?", Util.GetString("Yes"), Util.GetString("Cancel"));
                var clearDataResult = await clearDataConfirmationDialog.ShowAsync();
                if (clearDataResult == ContentDialogResult.Primary)
                {
                    Task clearData = FrameworkHelper.ClearData();
                    await clearData;
                }
            }
            catch (Exception ex)
            {
                Logger.E($"Exception caught while clearing the application data. Message {ex.Message}");
            }
        }

    }
}
