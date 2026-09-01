using Newtonsoft.Json;
using System.Net;

namespace ApiClient
{
    // Einziger Weg, wie die WPF-App an Sensordaten kommt: HTTP-Aufrufe gegen die
    // Database-API (ASP.NET). Kein direkter Datenbankzugriff auf sensordata.
    public class SensorReadingsApiClient(HttpClient httpClient) : ISensorReadingsApiClient
    {
        private readonly HttpClient _httpClient = httpClient;
        const string _baseUrl = "api/sensorreadings";

        public async Task<List<SensorReadingDto>> GetAllAsync(string? deviceId = null, int take = 100)
        {
            var url = $"{_baseUrl}?take={take}";
            if (!string.IsNullOrWhiteSpace(deviceId))
            {
                url += $"&deviceId={Uri.EscapeDataString(deviceId)}";
            }

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"SensorReadings, GetAllAsync, Error {response.StatusCode}, {response.ReasonPhrase}");
            }

            var contentTemp = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<List<SensorReadingDto>>(contentTemp) ?? [];
            return result;
        }

        public async Task<SensorReadingDto?> GetLatestAsync(string? deviceId = null)
        {
            var url = $"{_baseUrl}/latest";
            if (!string.IsNullOrWhiteSpace(deviceId))
            {
                url += $"?deviceId={Uri.EscapeDataString(deviceId)}";
            }

            var response = await _httpClient.GetAsync(url);
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"SensorReadings, GetLatestAsync, Error {response.StatusCode}, {response.ReasonPhrase}");
            }

            var contentTemp = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<SensorReadingDto>(contentTemp);
        }
    }
}
