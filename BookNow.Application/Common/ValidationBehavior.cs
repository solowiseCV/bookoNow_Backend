using BookNow.Domain.Common;
using FluentValidation;
using MediatR;

namespace BookNow.Application.Common;

public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) 
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);
            var validationResults = await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, cancellationToken)));
            var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();

            if (failures.Count != 0)
            {
                var errors = failures.Select(f => f.ErrorMessage).ToList();
                
                // We assume TResponse is a Result<T> type
                var resultType = typeof(TResponse);
                if (resultType.IsGenericType && resultType.GetGenericTypeDefinition() == typeof(Result<>))
                {
                    var failureMethod = resultType.GetMethod("Failure", new[] { typeof(string), typeof(IEnumerable<string>) });
                    if (failureMethod != null)
                    {
                        var failureResult = failureMethod.Invoke(null, new object[] { "Validation failed.", errors });
                        return (TResponse)failureResult!;
                    }
                }
                
                throw new ValidationException(failures);
            }
        }
        return await next();
    }
}
