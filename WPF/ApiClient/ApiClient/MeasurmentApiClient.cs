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
        const string _baseUrl = "http://192.168.68.56:5226/api/Measurments";

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
