using ApiClient.Contracts;
using ApiClient.Dtos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ApiClient.ApiClient
{
    public class CityWeatherApiClient(HttpClient httpClient): ICityWeatherApiClient
    {
        private readonly HttpClient _httpClient = httpClient;
     
        const string _MeteoUrl = "https://geocoding-api.open-meteo.com/v1/search?name=";
        const string _WeatherApiUril = "https://api.open-meteo.com/v1/forecast?latitude=";

        public async Task<MeteoParameterRecord?> GetMeteoParameterAsync(string name)
        {
            var response = await _httpClient.GetAsync($"{_MeteoUrl}{name}&count=1");
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Open-Meteo-Parameter, Error {response.StatusCode}, {response.ReasonPhrase}");
            }

            var contentTemp = await response.Content.ReadAsStringAsync();
            var geocoding = JsonConvert.DeserializeObject<GeocodingResponse?>(contentTemp);
            return geocoding?.results.Length > 0 ? geocoding.results[0] : null;
        }

        public async Task<CityWeatherDto> GetCityWeatherByNameAsync(string cityName)
        {
            var cityValue = await GetMeteoParameterAsync(cityName);
            if (cityValue == null)
            {
                throw new Exception($"{cityName} not found");
            }

            double lat = cityValue.latitude;
            double lon = cityValue.longitude;
            var response = await _httpClient.GetAsync($"{_WeatherApiUril}{lat}&longitude={lon}&daily=temperature_2m_max,temperature_2m_min&timezone=auto");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"CityWeather, Error {response.StatusCode}, {response.ReasonPhrase}");
            }

            var contentTemp = await response.Content.ReadAsStringAsync();
            var forecast = JsonConvert.DeserializeObject<ForecastResponse>(contentTemp);
            if(forecast == null || forecast.daily.temperature_2m_max.Count == 0)
            {
                throw new Exception($"No data for {cityName}");
            }

            return new CityWeatherDto
            {
                Name = cityValue.name,
                Latitude = lat,
                Longitude = lon,
                MaxTemp = forecast.daily.temperature_2m_max[0],
                MinTemp = forecast.daily.temperature_2m_min[0],
                LastUpdate = DateTime.UtcNow
            };

        }

        


        public record MeteoParameterRecord
            (
                int id, string name, double latitude, double longitude
            );

        public record GeocodingResponse(MeteoParameterRecord[] results);


        public record DailyWeather
            (
                List<string>time,
                List<double> temperature_2m_max,
                List<double> temperature_2m_min
            );

        public record ForecastResponse(DailyWeather daily);
    }
}
