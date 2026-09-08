using ApiClient.Contracts;
using ApiClient.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace ApiClient.ApiClient
{
    public class SensorApiClient(HttpClient httpClient) :ISensorApiClient
    {
        private readonly HttpClient _httpClient = httpClient;
        const string _baseUrl = "https://webapi20260907135900-a8g7dybugngfh0bk.westus3-01.azurewebsites.net/api/Sensors";

        public async Task<IEnumerable<SensorDto>> GetAllAsync()
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/GetAll");
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Sensors, GetAll, Error {response.StatusCode}, {response.ReasonPhrase}");
            }

            var contentTemp = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<IEnumerable<SensorDto>>(contentTemp) ?? [];
            return result;
        }

      
    }
}
