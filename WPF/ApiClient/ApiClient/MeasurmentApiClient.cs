using ApiClient.Contracts;
using ApiClient.Dtos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ApiClient.ApiClient
{
    public class MeasurmentApiClient(HttpClient httpClient):IMeasurmentApiClient
    {
        private readonly HttpClient _httpClient = httpClient;
        const string _baseUrl = "https://webapi20260907135900-a8g7dybugngfh0bk.westus3-01.azurewebsites.net/api/Measurments";

        public async Task<IEnumerable<MeasurmentDto>> GetAllAsync()
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/GetAll");
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Measuremnts, GetAll, Error {response.StatusCode}, {response.ReasonPhrase}");
            }

            var contentTemp = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<IEnumerable<MeasurmentDto>>(contentTemp) ?? [];
            return result;
        }




    }
}
