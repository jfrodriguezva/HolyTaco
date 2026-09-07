using Microsoft.Data.SqlClient;
using Polly;
using Polly.Retry;

namespace HolyTac.Orders.Infrastructure;

public static class ResiliencePolicies
{
    public static readonly ResiliencePipeline DatabasePipeline = new ResiliencePipelineBuilder()
        .AddRetry(new RetryStrategyOptions
        {
            ShouldHandle = new PredicateBuilder().Handle<SqlException>().Handle<TimeoutException>(),
            MaxRetryAttempts = 3,
            BackoffType = DelayBackoffType.Exponential,
            Delay = TimeSpan.FromMilliseconds(200)
        })
        .Build();
}
