using System;

namespace FooBar.Domain.Exceptions
{
    public class GeneralException : Exception
    {
        public GeneralException(string message) : base(message)
        {
        }
    }
}