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
    public class MeasurmentApiClient(HttpClient httpClient, string? apiKey = null):IMeasurmentApiClient
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly string? _apiKey = apiKey;
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

        public async Task<int> CleanupAsync()
        {
            using var request = new HttpRequestMessage(HttpMethod.Delete, $"{_baseUrl}/Cleanup");
            if (!string.IsNullOrEmpty(_apiKey))
            {
                request.Headers.Add("X-Api-Key", _apiKey);
            }

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Measuremnts, Cleanup, Error {response.StatusCode}, {response.ReasonPhrase}");
            }

            var contentTemp = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<int>(contentTemp);
        }

    }
}
