using Azure;
using Azure.Data.Tables;

namespace Functions;

public class SensorDataTableItem : ITableEntity
{
    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
    public int SensorId { get; set; }
    public decimal SensorValue { get; set; }
    public DateTimeOffset EventTime { get; set; }

    public SensorDataTableItem(SensorData data)
    {
        PartitionKey = data.SensorId.ToString();
        RowKey = data.EventTime.ToString("yyyyMMddHHmmss");
        SensorId = data.SensorId;
        SensorValue = data.SensorValue;
        EventTime = data.EventTime;
    }
    
    public SensorDataTableItem()
    {
    }
}