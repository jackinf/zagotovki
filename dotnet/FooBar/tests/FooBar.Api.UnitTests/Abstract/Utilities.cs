using System.Collections.Generic;
using System.Linq;
using FluentAssertions.Specialized;
using FluentValidation;

namespace FooBar.Api.UnitTests.Abstract
{
    public static class Utilities
    {
        public static List<string> ExtractErrorMessages(this ExceptionAssertions<ValidationException> exceptionAssertions)
            => exceptionAssertions.Subject
                .SelectMany(x => x.Errors)
                .Select(x => x.ErrorMessage)
                .ToList();
    }
}