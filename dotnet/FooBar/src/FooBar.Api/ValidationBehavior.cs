// using System.Collections.Generic;
// using System.Linq;
// using System.Threading;
// using System.Threading.Tasks;
// using FluentValidation;
// using FluentValidation.Results;
// using MediatR;
//
// namespace FooBar.Api
// {
//     public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
//         where TRequest : IRequest<TResponse>
//     {
//         private readonly IEnumerable<IValidator<TRequest>> validators;
//
//         public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
//         {
//             this.validators = validators;
//         }
//
//         public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
//         {
//             var context = new ValidationContext(request);
//             var validationResults = new List<ValidationResult>();
//             foreach (var validator in validators)
//             {
//                 var validationResult = await validator.ValidateAsync(context, cancellationToken);
//                 validationResults.Add(validationResult);
//             }
//             
//             var failures = validationResults
//                 .SelectMany(result => result.Errors)
//                 .Where(f => f != null)
//                 .ToList();
//
//             if (failures.Any())
//             {
//                 throw new ValidationException(failures);
//             }
//
//             return await next();
//         }
//     }
// }