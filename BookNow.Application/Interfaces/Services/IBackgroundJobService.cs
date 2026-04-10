using System.Linq.Expressions;

namespace BookNow.Application.Interfaces.Services;

public interface IBackgroundJobService
{
    string Enqueue(Expression<Action> methodCall);
    string Enqueue<T>(Expression<Action<T>> methodCall);
    string Enqueue(Expression<Func<Task>> methodCall);
    string Enqueue<T>(Expression<Func<T, Task>> methodCall);
    
    // Support for delayed/scheduled jobs if needed later
    string Schedule(Expression<Action> methodCall, TimeSpan delay);
    string Schedule<T>(Expression<Action<T>> methodCall, TimeSpan delay);
}
