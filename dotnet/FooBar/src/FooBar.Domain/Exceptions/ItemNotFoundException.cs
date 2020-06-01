using System;

namespace FooBar.Domain.Exceptions
{
    public class ItemNotFoundException : Exception
    {
        public ItemNotFoundException(string message) : base(message)
        {
        }
    }
}