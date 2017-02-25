using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Weather.Models;

namespace Weather.ViewModels
{
    public class WeatherAppViewModel
    {
        public IpLocationObject IpLocationObject { get; set; }
        public WeatherObject WeatherObject { get; set; }
        public string ImageUrl { get; set; }

    }
}