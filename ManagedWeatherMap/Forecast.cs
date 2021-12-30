﻿/*
 * MIT License
 *
 * Copyright (c) 2021 EoflaOE and its companies
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 * 
 */

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using Extensification.DictionaryExts;
using Newtonsoft.Json.Linq;

namespace Core
{
    public static partial class Forecast
    {

        /// <summary>
        /// Gets current weather info from OpenWeatherMap
        /// </summary>
        /// <param name="CityID">City ID</param>
        /// <param name="APIKey">API key</param>
        /// <returns>A class containing properties of weather information</returns>
        public static ForecastInfo GetWeatherInfo(long CityID, string APIKey, UnitMeasurement Unit = UnitMeasurement.Metric)
        {
            // TODO: Use HttpClient if possible. Not available on .NET Framework 4.8, so use compiler constants to change behavior.
            ForecastInfo WeatherInfo = new ForecastInfo() { CityID = CityID, TemperatureMeasurement = Unit };
            string WeatherURL = $"http://api.openweathermap.org/data/2.5/weather?id={CityID}&appid={APIKey}";
            WebClient WeatherDownloader = new WebClient();
            string WeatherData;
            JToken WeatherToken;
            Debug.WriteLine("Made new instance of class with {0} and {1}", CityID, Unit);
            Debug.WriteLine("Weather URL: {0}", WeatherURL);

            // Deal with measurements
            if (Unit == UnitMeasurement.Imperial)
            {
                WeatherURL += "&units=imperial";
            }
            else
            {
                WeatherURL += "&units=metric";
            }

            // Download and parse JSON data
            WeatherData = WeatherDownloader.DownloadString(WeatherURL);
            WeatherToken = JToken.Parse(WeatherData);

            // Put needed data to the class
            WeatherInfo.Weather = (WeatherCondition)WeatherToken.SelectToken("weather").First.SelectToken("id").ToObject(typeof(WeatherCondition));
            WeatherInfo.Temperature = (double)WeatherToken.SelectToken("main").SelectToken("temp").ToObject(typeof(double));
            WeatherInfo.FeelsLike = (double)WeatherToken.SelectToken("main").SelectToken("feels_like").ToObject(typeof(double));
            WeatherInfo.Pressure = (double)WeatherToken.SelectToken("main").SelectToken("pressure").ToObject(typeof(double));
            WeatherInfo.Humidity = (double)WeatherToken.SelectToken("main").SelectToken("humidity").ToObject(typeof(double));
            WeatherInfo.WindSpeed = (double)WeatherToken.SelectToken("wind").SelectToken("speed").ToObject(typeof(double));
            WeatherInfo.WindDirection = (double)WeatherToken.SelectToken("wind").SelectToken("deg").ToObject(typeof(double));
            WeatherInfo.CityName = WeatherToken.SelectToken("name").ToString();
            return WeatherInfo;
        }

        /// <summary>
        /// Gets current weather info from OpenWeatherMap
        /// </summary>
        /// <param name="CityName">City name</param>
        /// <param name="APIKey">API Key</param>
        /// <returns>A class containing properties of weather information</returns>
        public static ForecastInfo GetWeatherInfo(string CityName, string APIKey, UnitMeasurement Unit = UnitMeasurement.Metric)
        {
            var WeatherInfo = new ForecastInfo() { CityName = CityName, TemperatureMeasurement = Unit };
            string WeatherURL = $"http://api.openweathermap.org/data/2.5/weather?q={CityName}&appid={APIKey}";
            var WeatherDownloader = new WebClient();
            string WeatherData;
            JToken WeatherToken;
            Debug.WriteLine("Made new instance of class with {0} and {1}", CityName, Unit);
            Debug.WriteLine("Weather URL: {0}", WeatherURL);

            // Deal with measurements
            if (Unit == UnitMeasurement.Imperial)
            {
                WeatherURL += "&units=imperial";
            }
            else
            {
                WeatherURL += "&units=metric";
            }

            // Download and parse JSON data
            WeatherData = WeatherDownloader.DownloadString(WeatherURL);
            WeatherToken = JToken.Parse(WeatherData);

            // Put needed data to the class
            WeatherInfo.Weather = (WeatherCondition)WeatherToken.SelectToken("weather").First.SelectToken("id").ToObject(typeof(WeatherCondition));
            WeatherInfo.Temperature = (double)WeatherToken.SelectToken("main").SelectToken("temp").ToObject(typeof(double));
            WeatherInfo.FeelsLike = (double)WeatherToken.SelectToken("main").SelectToken("feels_like").ToObject(typeof(double));
            WeatherInfo.Pressure = (double)WeatherToken.SelectToken("main").SelectToken("pressure").ToObject(typeof(double));
            WeatherInfo.Humidity = (double)WeatherToken.SelectToken("main").SelectToken("humidity").ToObject(typeof(double));
            WeatherInfo.WindSpeed = (double)WeatherToken.SelectToken("wind").SelectToken("speed").ToObject(typeof(double));
            WeatherInfo.WindDirection = (double)WeatherToken.SelectToken("wind").SelectToken("deg").ToObject(typeof(double));
            WeatherInfo.CityID = (long)WeatherToken.SelectToken("id").ToObject(typeof(long));
            return WeatherInfo;
        }

        /// <summary>
        /// Lists all the available cities
        /// </summary>
        public static Dictionary<long, string> ListAllCities()
        {
            string WeatherCityListURL = $"http://bulk.openweathermap.org/sample/city.list.json.gz";
            var WeatherCityListDownloader = new WebClient();
            GZipStream WeatherCityListData;
            Stream WeatherCityListDataStream;
            var WeatherCityListUncompressed = new List<byte>();
            int WeatherCityListReadByte = 0;
            JToken WeatherCityListToken;
            var WeatherCityList = new Dictionary<long, string>();
            Debug.WriteLine("Weather City List URL: {0}", WeatherCityListURL);

            // Download and parse JSON data
            WeatherCityListDataStream = WeatherCityListDownloader.OpenRead(WeatherCityListURL);
            WeatherCityListData = new GZipStream(WeatherCityListDataStream, CompressionMode.Decompress, false);
            while (WeatherCityListReadByte != -1)
            {
                WeatherCityListReadByte = WeatherCityListData.ReadByte();
                if (WeatherCityListReadByte != -1)
                    WeatherCityListUncompressed.Add((byte)WeatherCityListReadByte);
            }

            WeatherCityListToken = JToken.Parse(Encoding.Default.GetString(WeatherCityListUncompressed.ToArray()));

            // Put needed data to the class
            foreach (JToken WeatherCityToken in WeatherCityListToken)
                WeatherCityList.AddIfNotFound((long)WeatherCityToken["id"], (string)WeatherCityToken["name"]);

            // Return list
            return WeatherCityList;
        }
    }
}