namespace ApiClient
{
    public interface ISensorReadingsApiClient
    {
        Task<List<SensorReadingDto>> GetAllAsync(string? deviceId = null, int take = 100);
        Task<SensorReadingDto?> GetLatestAsync(string? deviceId = null);
    }
}
