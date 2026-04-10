using System.Linq.Expressions;
using BookNow.Application.Interfaces.Services;
using Hangfire;

namespace BookNow.Infrastructure.Services;

public class HangfireBackgroundJobService(IBackgroundJobClient backgroundJobClient) : IBackgroundJobService
{
    public string Enqueue(Expression<Action> methodCall)
    {
        return backgroundJobClient.Enqueue(methodCall);
    }

    public string Enqueue<T>(Expression<Action<T>> methodCall)
    {
        return backgroundJobClient.Enqueue(methodCall);
    }

    public string Enqueue(Expression<Func<Task>> methodCall)
    {
        return backgroundJobClient.Enqueue(methodCall);
    }

    public string Enqueue<T>(Expression<Func<T, Task>> methodCall)
    {
        return backgroundJobClient.Enqueue(methodCall);
    }

    public string Schedule(Expression<Action> methodCall, TimeSpan delay)
    {
        return backgroundJobClient.Schedule(methodCall, delay);
    }

    public string Schedule<T>(Expression<Action<T>> methodCall, TimeSpan delay)
    {
        return backgroundJobClient.Schedule(methodCall, delay);
    }
}
