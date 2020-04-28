//	Generated using Unvired Modeller - Build R-4.000.0120

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Unvired.Kernel.SQLite;
using Unvired.Kernel.Model;
using Unvired.Kernel.UWP.Model;

namespace Entity
{
    // This class is part of the BE "WEATHER". 
    public partial class WEATHER_HEADER : DataStructure
    {

        // City
        private string _CITY;

        // Weather
        private string _WEATHER_DESC;

        // Temperature
        private string _TEMPERATURE;

        // Humidity
        private string _HUMIDITY;

        internal static bool IsHeader
        {
            get
            {
                return true;
            }
        }

        [Unique]
        public string CITY
        {
            get
            {
                return _CITY;
            }
            set
            {
                _CITY = value;
                RaisePropertyChanged("CITY");
            }
        }

        public string WEATHER_DESC
        {
            get
            {
                return _WEATHER_DESC;
            }
            set
            {
                _WEATHER_DESC = value;
                RaisePropertyChanged("WEATHER_DESC");
            }
        }

        public string TEMPERATURE
        {
            get
            {
                return _TEMPERATURE;
            }
            set
            {
                _TEMPERATURE = value;
                RaisePropertyChanged("TEMPERATURE");
            }
        }

        public string HUMIDITY
        {
            get
            {
                return _HUMIDITY;
            }
            set
            {
                _HUMIDITY = value;
                RaisePropertyChanged("HUMIDITY");
            }
        }
    }
}