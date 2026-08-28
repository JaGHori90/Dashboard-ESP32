using System.Net;
using System.Net.Http.Json;

namespace ApiClient
{
    // Einziger Weg, wie die WPF-App an Sensordaten kommt: HTTP-Aufrufe gegen die
    // Database-API (ASP.NET). Kein direkter Datenbankzugriff auf sensordata.
    public class SensorReadingsApiClient
    {
        private readonly HttpClient _httpClient;

        public SensorReadingsApiClient(string apiBaseUrl)
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(apiBaseUrl) };
        }

        public async Task<List<SensorReadingDto>> GetAllAsync(string? deviceId = null, int take = 100)
        {
            var url = $"api/sensorreadings?take={take}";
            if (!string.IsNullOrWhiteSpace(deviceId))
            {
                url += $"&deviceId={Uri.EscapeDataString(deviceId)}";
            }

            var readings = await _httpClient.GetFromJsonAsync<List<SensorReadingDto>>(url);
            return readings ?? new List<SensorReadingDto>();
        }

        public async Task<SensorReadingDto?> GetLatestAsync(string? deviceId = null)
        {
            var url = "api/sensorreadings/latest";
            if (!string.IsNullOrWhiteSpace(deviceId))
            {
                url += $"?deviceId={Uri.EscapeDataString(deviceId)}";
            }

            var response = await _httpClient.GetAsync(url);
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<SensorReadingDto>();
        }
    }
}
