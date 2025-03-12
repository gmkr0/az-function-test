using Functions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using FromBodyAttribute = Microsoft.Azure.Functions.Worker.Http.FromBodyAttribute;

namespace first_function.SensorData
{
    public class HandleRequest
    {
        private readonly ILogger<HandleRequest> _logger;
        private readonly ISensorDataRepository _repository;

        public HandleRequest(ILogger<HandleRequest> logger, ISensorDataRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        [Function("PostSensorData")]
        public async Task<IActionResult> PostSensorData(
            [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req,
            [FromBody] Functions.SensorData? data)
        {
            if (data?.IsValid() != true)
                return new BadRequestResult();
                
            await _repository.SaveSensorDataAsync(data);
            return new OkResult();
        }
        
        [Function("GetSensorData")]
        public async Task<IActionResult> GetSensorData(
            [HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
        {
            req.Query.TryGetValue("sensorId", out var sensorId);
            if (!int.TryParse(sensorId, out var id))
                return new BadRequestResult();
            var result = await _repository.GetSensorDataAsync(id);
            return new ObjectResult(result);
        }
    }
}
