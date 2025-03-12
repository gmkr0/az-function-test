namespace Functions;

public interface ISensorDataRepository
{
    public Task<List<SensorData>> GetSensorDataAsync(int sensorId, CancellationToken cancellationToken = default);
    public Task SaveSensorDataAsync(SensorData sensorData, CancellationToken cancellationToken = default);
}