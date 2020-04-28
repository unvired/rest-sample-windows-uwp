using Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Unvired.Kernel.UWP.Sync;
using UNVIRED_REST_SAMPLE.Utility;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

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
        private WEATHER_HEADER _weatherHeader;
        public WEATHER_HEADER WeatherHeader
        {
            get => _weatherHeader;
            set
            {
                _weatherHeader = value;
                RaisePropertyChanged("WeatherHeader");
            }
        }

        public Weather()
        {
            this.InitializeComponent();
            if (WeatherHeader == null) WeatherHeader = new WEATHER_HEADER();
            ValidationTextBlock.Visibility = Visibility.Collapsed;
        }

        private async void GetWeatherClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(WeatherHeader.CITY)) { ValidationTextBlock.Visibility = Visibility.Visible; return; }
            if (!ConnectionHelper.HasAnyInternetConnection())
            {
                var connectionDialog = Util.AlertContentDialog("No Internet", "No Internet ConnectionPlease Try Again Later", "OK");
                await connectionDialog.ShowAsync();
                return;
            }
            Util.ShowProgressDialog();
        }

        private void CityName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ValidationTextBlock.Visibility == Visibility.Visible)
                ValidationTextBlock.Visibility = Visibility.Collapsed;

        }
    }
}
