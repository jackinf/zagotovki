using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;

namespace FooBar.Api.Features.CatalogItems.Add
{
    public class AddCatalogItemValidator : AbstractValidator<AddCatalogItem>
    {
        public override Task<ValidationResult> ValidateAsync(ValidationContext<AddCatalogItem> context, CancellationToken cancellation = new CancellationToken())
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Price).GreaterThan(0);
            
            return base.ValidateAsync(context, cancellation);
        }
    }
}