using System.Reflection.Metadata;
using backend.person.api.Services;
using Microsoft.AspNetCore.Mvc;
using Polly;
using Polly.Retry;
using Serilog;

namespace backend.person.api.Controller;

[ApiController]
[Route("[controller]")]
public class CostumerController: ControllerBase
{
    private readonly ICostumerService _costumerService;

    public CostumerController(ICostumerService costumerService)
    {
        _costumerService = costumerService;
    }

    [HttpGet]
    public IActionResult GetCostumer()
    {
        try
        {
            var pipeline = new ResiliencePipelineBuilder<string>()
                .AddRetry(new RetryStrategyOptions<string>()
                {
                    MaxRetryAttempts = 4,
                    BackoffType = DelayBackoffType.Constant,
                    Delay = TimeSpan.FromSeconds(5),
                    ShouldHandle = new PredicateBuilder<string>()
                        .Handle<Exception>(),
                    OnRetry = arguments =>
                    {
                        Log.Error("Current Attempt: {Arg1}, {Arg2}", arguments.AttemptNumber, arguments.Outcome.Exception?.Message);
                        return ValueTask.CompletedTask;
                    }
                }).Build();
            
            

            pipeline.Execute( _costumerService.SimulatedMethod);
            
            Policy.Handle<Exception>()
                .WaitAndRetry(5, (count) => TimeSpan.FromSeconds(count))
                .Execute(_costumerService.SimulatedMethod);
            
            
            return Ok();

        } catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    
}