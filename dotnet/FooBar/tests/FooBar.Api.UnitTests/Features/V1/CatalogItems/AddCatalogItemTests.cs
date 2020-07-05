using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation;
using FooBar.Api.Features.V1.CatalogItems.Add;
using FooBar.Api.UnitTests.Abstract;
using FooBar.Domain.Entities;
using FooBar.Domain.Exceptions;
using FooBar.Domain.Interfaces;
using Moq;
using Stashbox.Mocking.Moq;
using Xunit;

namespace FooBar.Api.UnitTests.Features.V1.CatalogItems
{
    public class AddCatalogItemHandlerTests
    {
        private readonly AddCatalogItemHandler _sut;
        private readonly Mock<ICatalogItemRepository> _mockRepository;
        private readonly Mock<IAsyncRepository<CatalogBrand>> _mockCatalogBrandRepository;
        private readonly Mock<IAsyncRepository<CatalogType>> _mockCatalogTypeRepository;

        private class TestCatalogItem : CatalogItem
        {
            public TestCatalogItem(int id, int catalogTypeId, int catalogBrandId, string description, string name, decimal price, string pictureUri) 
                : base(catalogTypeId, catalogBrandId, description, name, price, pictureUri)
            {
                Id = id;
            }
        }
        
        public AddCatalogItemHandlerTests()
        {
            var stash = StashMoq.Create();
            _mockRepository = stash.Mock<ICatalogItemRepository>();
            _mockCatalogBrandRepository = stash.Mock<IAsyncRepository<CatalogBrand>>();
            _mockCatalogTypeRepository = stash.Mock<IAsyncRepository<CatalogType>>();

            _sut = stash
                .GetWithParamOverrides<AddCatalogItemHandler>(_mockRepository, _mockCatalogBrandRepository, _mockCatalogTypeRepository);
        }

        [Fact]
        public async Task WhenSendingInvalidData_ShouldThrow()
        {
            // Arrange
            const int catalogTypeId = 1;
            const int catalogBrandId = 2;
            var addCatalogItem = new AddCatalogItem("", "test-description", 123.45m, "picture-uri", catalogTypeId, catalogBrandId);
            MockGetByIdAsync(catalogTypeId, catalogBrandId, null, null);
            
            // Act
            var task = new Func<Task<int>>(() => _sut.Handle(addCatalogItem, CancellationToken.None));
            
            // Assert
            var exceptionAssertions = await task.Should().ThrowAsync<ValidationException>();
            var errors = exceptionAssertions.ExtractErrorMessages();
            errors.Should().Contain($"Catalog Brand with id {catalogBrandId} does not exist in DB");
            errors.Should().Contain($"Catalog Type with id {catalogTypeId} does not exist in DB");
        }

        [Fact]
        public async Task WhenFailingToAdd_ShouldThrow()
        {
            // Arrange
            const int catalogTypeId = 1;
            const int catalogBrandId = 2;
            var addCatalogItem = new AddCatalogItem("", "test-description", 123.45m, "picture-uri", catalogTypeId, catalogBrandId);
            MockGetByIdAsync(catalogTypeId, catalogBrandId, new CatalogType("type-1"), new CatalogBrand("brand-1"));
            _mockRepository
                .Setup(x => x.AddAsync(It.IsAny<CatalogItem>()))
                .ThrowsAsync(new Exception("db failure"));
            
            // Act
            var task = new Func<Task<int>>(() => _sut.Handle(addCatalogItem, CancellationToken.None));
            
            // Assert
            var exceptionAssertions = await task.Should().ThrowAsync<GeneralException>();
            var errors = exceptionAssertions.Subject
                .Select(x => x.Message)
                .ToList();
            errors.Should().Contain($"Failed to add an item");
        }

        [Fact]
        public async Task WhenSendingValidData_ShouldAddSuccessfully()
        {
            // Arrange
            const int catalogTypeId = 1;
            const int catalogBrandId = 2;
            const int catalogItemId = 3;
            var addCatalogItem = new AddCatalogItem("test-name", "test-description", 123.45m, "picture-uri", catalogTypeId, catalogBrandId);
            var addedCatalogItem = new TestCatalogItem(catalogItemId, catalogTypeId, catalogBrandId, "test-description", "test-name", 123.45m, "picture-uri");
            MockGetByIdAsync(catalogTypeId, catalogBrandId, new CatalogType("type-1"), new CatalogBrand("brand-1"));
            _mockRepository
                .Setup(x => x.AddAsync(It.IsAny<CatalogItem>()))
                .ReturnsAsync(addedCatalogItem);
            
            // Act
            var id = await _sut.Handle(addCatalogItem, CancellationToken.None);
            
            // Assert
            id.Should().Be(catalogItemId);
        }

        [Theory]
        [InlineData("", 123, "'Name' must not be empty.")]
        [InlineData("test", -1, "'Price' must be greater than '0'.")]
        public async Task ShouldValidate(string name, decimal price, string errorMessage)
        {
            var addCatalogItemValidator = new AddCatalogItemValidator();
            var result = await addCatalogItemValidator.ValidateAsync(new AddCatalogItem(name, "d", price, "p", 1, 2));
            result.IsValid.Should().Be(false);
            var errorsInline = string.Join(", ", result.Errors
                .Select(x => x.ErrorMessage)
                .OrderBy(x => x));
            errorsInline.Should().Be(errorMessage);
        }

        private void MockGetByIdAsync(int catalogTypeId, int catalogBrandId, CatalogType catalogType, CatalogBrand catalogBrand)
        {
            _mockCatalogTypeRepository
                .Setup(x => x.GetByIdAsync(catalogTypeId))
                .ReturnsAsync(catalogType);
            _mockCatalogBrandRepository
                .Setup(x => x.GetByIdAsync(catalogBrandId))
                .ReturnsAsync(catalogBrand);
        }
    }
}