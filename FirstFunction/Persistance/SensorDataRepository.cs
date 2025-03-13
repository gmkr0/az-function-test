using Azure.Data.Tables;
using GmkrFunctions;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace Functions;

public class SensorDataRepository : ISensorDataRepository
{
    private readonly ResiliencePipeline _resiliencePipeline;
    private readonly ILogger<SensorDataRepository> _logger;
    
    public SensorDataRepository(ILogger<SensorDataRepository> logger)
    {
        _logger = logger;
        _resiliencePipeline = new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions()
            {
                BackoffType = DelayBackoffType.Exponential,
                Delay = TimeSpan.FromMilliseconds(200),
                MaxDelay = TimeSpan.FromSeconds(3),
                MaxRetryAttempts = 5
            })
            .AddTimeout(TimeSpan.FromSeconds(10))
            .Build();
    }
        
    private async Task<TableClient> CreateTableClientAsync()
    {
        var table = new TableClient(
            Environment.GetEnvironmentVariable(Constants.EnvironmentVariables.AzureWebJobsStorage),
            Environment.GetEnvironmentVariable(Constants.EnvironmentVariables.SensorDataTable));
        _logger.LogInformation("Creating table {TableName} if not exists", table.Name);
        await table.CreateIfNotExistsAsync();
        return table;
    }

    public async Task<List<SensorData>> GetSensorDataAsync(int sensorId, CancellationToken cancellationToken = default)
    {
        var result = new List<SensorData>();
        await _resiliencePipeline.ExecuteAsync(async token =>
        {
            var client = await CreateTableClientAsync();
            var pages = client
                .QueryAsync<SensorDataTableItem>(q => q.PartitionKey == sensorId.ToString(),
                    cancellationToken: cancellationToken)
                .AsPages();
            await foreach (var page in pages)
            {
                result.AddRange(page.Values.Select(x => new SensorData
                {
                    SensorId = x.SensorId,
                    SensorValue = x.SensorValue,
                    EventTime = x.EventTime
                }));
            }

        }, cancellationToken:cancellationToken);
        return result;
    }

    public async Task SaveSensorDataAsync(SensorData sensorData, CancellationToken cancellationToken = default)
    {
        await _resiliencePipeline.ExecuteAsync(async token =>
        {
            var client = await CreateTableClientAsync();
            await client.AddEntityAsync(new SensorDataTableItem(sensorData), cancellationToken: cancellationToken);
        }, cancellationToken:cancellationToken);
    }
}