using FluentAssertions;
using Functions;
using GmkrFunctions.SensorData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Tests;

public class FunctionTests
{
    private readonly HandleRequest _function;

    public FunctionTests()
    {
        _function = new HandleRequest(Mock.Of<ISensorDataRepository>());
    }
    
    [Fact]
    public async Task Post_Checks_Contract()
    {
        var result = await _function
            .PostSensorData(Mock.Of<HttpRequest>(), new SensorData());
        result.Should().BeOfType<BadRequestResult>();
    }
}