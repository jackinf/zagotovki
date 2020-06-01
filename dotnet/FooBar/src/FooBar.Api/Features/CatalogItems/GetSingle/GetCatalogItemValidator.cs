using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;

namespace FooBar.Api.Features.CatalogItems.GetSingle
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