using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;

namespace FooBar.Api.Features.V1.CatalogItems.GetSingle
{
    public class GetCatalogItemValidator : AbstractValidator<GetCatalogItem>
    {
        public override Task<ValidationResult> ValidateAsync(ValidationContext<GetCatalogItem> context, CancellationToken cancellation = new CancellationToken())
        {
            RuleFor(x => x.Id).GreaterThan(0);
            
            return base.ValidateAsync(context, cancellation);
        }
    }
}