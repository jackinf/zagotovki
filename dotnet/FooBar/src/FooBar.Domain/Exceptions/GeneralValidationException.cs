using System;

namespace FooBar.Domain.Exceptions
{
    public class GeneralValidationException : Exception
    {
        public GeneralValidationException(string message) : base(message)
        {
        }
    }
}