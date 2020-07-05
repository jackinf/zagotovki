﻿using Xunit;

 namespace FooBar.Api.UnitTests.Abstract
{
    public abstract class TestBase<T> : IClassFixture<T> where T : ServiceFixture
    {
        public T ServiceFixture;

        protected TestBase(T serviceFixture)
        {
            ServiceFixture = serviceFixture;
        }
    }
}
