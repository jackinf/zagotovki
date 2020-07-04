using System;

namespace FooBar.Domain.Exceptions
{
    public class MissingConfigurationException : Exception
    {
        public MissingConfigurationException(string message): base(message)
        {
        }
    }
}