using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Weather.Models;
using Weather.ViewModels;

namespace Weather.Controllers
{
    public class WeatherController : Controller
    {
        // GET: Weather
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult WeatherView()
        {
            ////////////////
            /// Geting the users IP Address

            var ipaddress = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipaddress))
                ipaddress = Request.ServerVariables["REMOTE_ADDR"];

            ///////////////
            /// Getting the location object
            /// 
            /// 
            string responseFromServer_Location;
            WebRequest request = WebRequest.Create("http://ipinfo.io/" + ipaddress + "/geo");
            WebResponse response = request.GetResponse();

            using (Stream dataStream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(dataStream))
            {
                responseFromServer_Location = reader.ReadToEnd();
            }




            //////////////////
            /// deserialize the JSon Object into peices oject 
            /// 


            var ipLocationObject = new IpLocationObject();

            ipLocationObject = JsonConvert.DeserializeObject<IpLocationObject>(responseFromServer_Location);



            ////use the ipLocation to get weather object from api using zipcode!
            /// 
            /// 
            string responseFromServer_Weather;
            
            WebRequest _request = WebRequest.Create("http://api.wunderground.com/api/9b16b668f3b79445/conditions/q/" + ipLocationObject.Postal + ".json");
            WebResponse _response = _request.GetResponse();
            using (Stream dataStream = _response.GetResponseStream())
            using (StreamReader reader = new StreamReader(dataStream))
            {
                responseFromServer_Weather = reader.ReadToEnd();
            }

            var weatherobject = new WeatherObject();



            dynamic rslts = JsonConvert.DeserializeObject<dynamic>(responseFromServer_Weather);
            weatherobject.temp_f = rslts.current_observation.temp_f;
            weatherobject.temp_c = rslts.current_observation.temp_c;
            weatherobject.percip_in = rslts.current_observation.precip_today_in;
            weatherobject.weather = rslts.current_observation.icon.ToString();

            // partly
            string testForPartly = new string(weatherobject.weather.Take(6).ToArray());


            if (testForPartly == "partly")
            {
                var wo = weatherobject.weather.Skip(6);
                string WeatherObject = new string(wo.ToArray());
                weatherobject.weather = WeatherObject;

            }


            /////////////////////////////////


            //THE WEATHER API IS RETURNING 'MOSTLY CLOUDY' NEEDS TO BE ONE STRING WITH NO SPACES TO FEED TO THE FLICKER API!//

            ///////////////////////////////////////////////////////////////////////////////
            string responseFromServer_Image;

            WebRequest image_request = WebRequest.Create("https://api.flickr.com/services/feeds/photos_public.gne?format=json&tags=beach,weather," + weatherobject.weather);
            WebResponse image_response = image_request.GetResponse();
            using (Stream dataStream = image_response.GetResponseStream())
            using (StreamReader reader = new StreamReader(dataStream))
            {
                responseFromServer_Image = reader.ReadToEnd();
            }

            var responseFromServer_Image_json_Array = responseFromServer_Image.Reverse().Skip(1).Reverse().Skip(15).ToArray();
            var responseFromServer_Image_Json_Formatted = new string(responseFromServer_Image_json_Array);



            dynamic image_rslts = JsonConvert.DeserializeObject<dynamic>(responseFromServer_Image_Json_Formatted);






            Random random = new Random();
            var randomNumber = random.Next(0, 10);

            string image = image_rslts.items[randomNumber].media.m;
            string imageA = image_rslts.items[randomNumber].media.m;


            /////////////////////////////////////////////////////////////////////
            var weatherAppViewModel = new WeatherAppViewModel()
            {
                WeatherObject = weatherobject,
                IpLocationObject = ipLocationObject,
                ImageUrl = image

            };







            return View(weatherAppViewModel);

        }
    }
}