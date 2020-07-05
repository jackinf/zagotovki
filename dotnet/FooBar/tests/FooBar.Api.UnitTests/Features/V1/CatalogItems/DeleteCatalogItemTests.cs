using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FooBar.Api.Features.V1.CatalogItems.Delete;
using FooBar.Domain.Entities;
using FooBar.Domain.Exceptions;
using FooBar.Domain.Interfaces;
using MediatR;
using Moq;
using Stashbox.Mocking.Moq;
using Xunit;

namespace FooBar.Api.UnitTests.Features.V1.CatalogItems
{
    public class DeleteCatalogItemTests
    {
        private readonly DeleteCatalogItemHandler _sut;
        private readonly Mock<ICatalogItemRepository> _mockRepository;

        public DeleteCatalogItemTests()
        {
            var stash = StashMoq.Create();
            _mockRepository = stash.Mock<ICatalogItemRepository>();

            _sut = stash.GetWithParamOverrides<DeleteCatalogItemHandler>(_mockRepository);
        }

        [Fact]
        public async Task WhenCannotFindItem_ShouldThrow()
        {
            // Arrange
            const int catalogItemId = 1;
            _mockRepository
                .Setup(x => x.GetByIdAsync(catalogItemId))
                .ReturnsAsync((CatalogItem) null);
            
            // Act
            var task = new Func<Task<Unit>>(() => _sut.Handle(new DeleteCatalogItem(1), CancellationToken.None));
            
            // Assert
            var exceptionAssertions = await task.Should().ThrowAsync<ItemNotFoundException>();
            var errors = exceptionAssertions.Subject.Select(x => x.Message);
            errors.Should().ContainSingle($"Catalog item with {catalogItemId} was not found");
        }

        [Fact]
        public async Task WhenFailingToSave_ShouldThrow()
        {
            // Arrange
            const int catalogItemId = 1;
            _mockRepository
                .Setup(x => x.GetByIdAsync(catalogItemId))
                .ReturnsAsync(new CatalogItem(1, 2, "test", "test", 1.2m, "p"));
            _mockRepository
                .Setup(x => x.DeleteAsync(It.IsAny<CatalogItem>()))
                .ThrowsAsync(new Exception("db failure"));
            
            // Act
            var task = new Func<Task<Unit>>(() => _sut.Handle(new DeleteCatalogItem(1), CancellationToken.None));
            
            // Assert
            var exceptionAssertions = await task.Should().ThrowAsync<GeneralException>();
            var errors = exceptionAssertions.Subject.Select(x => x.Message).ToList();
            errors.Should().ContainSingle($"Failed to delete an item with id {catalogItemId}");
        }

        [Fact]
        public async Task WhenSavingSuccessfully_ShouldSucceed()
        {
            // Arrange
            const int catalogItemId = 1;
            _mockRepository
                .Setup(x => x.GetByIdAsync(catalogItemId))
                .ReturnsAsync(new CatalogItem(1, 2, "test", "test", 1.2m, "p"));

            // Act
            var unit = await _sut.Handle(new DeleteCatalogItem(1), CancellationToken.None);
            
            unit.Should().BeEquivalentTo(new Unit());
        }
    }
}