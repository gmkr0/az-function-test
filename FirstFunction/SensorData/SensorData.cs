namespace Functions;

public class SensorData
{
    public int SensorId { get; set; }
    public decimal SensorValue { get; set; }
    public DateTimeOffset EventTime { get; set; }
    
    public bool IsValid() => SensorId != default && SensorValue != default;
}