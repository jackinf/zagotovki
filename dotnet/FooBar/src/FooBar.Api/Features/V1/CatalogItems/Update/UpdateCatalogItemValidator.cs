using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;

namespace FooBar.Api.Features.V1.CatalogItems.Update
{
    public class UpdateCatalogItemValidator : AbstractValidator<UpdateCatalogItem>
    {
        public override Task<ValidationResult> ValidateAsync(ValidationContext<UpdateCatalogItem> context, CancellationToken cancellation = new CancellationToken())
        {
            RuleFor(x => x.Id).GreaterThan(0);
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Price).GreaterThan(0);
            
            return base.ValidateAsync(context, cancellation);
        }
    }
}