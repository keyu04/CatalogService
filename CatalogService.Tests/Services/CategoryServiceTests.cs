using CatalogService.Common.DTOs;
using CatalogService.DTOs.Category;
using CatalogService.Models;
using CatalogService.Repository.Interfaces;
using CatalogService.Services.Implementations;
using Microsoft.Extensions.Logging;
using Moq;

namespace CatalogService.Tests.Services;

public class CategoryServiceTests
{
    private readonly Mock<ICategoryRepository>      _repoMock;
    private readonly Mock<ILogger<CategoryService>> _loggerMock;
    private readonly CategoryService                _service;

    public CategoryServiceTests()
    {
        _repoMock   = new Mock<ICategoryRepository>();
        _loggerMock = new Mock<ILogger<CategoryService>>();
        _service    = new CategoryService(_repoMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnPagedResult()
    {
        var fakeCategories = new List<Category>
        {
            new() { Id = Guid.NewGuid(), Name = "Fruits", Slug = "fruits" },
            new() { Id = Guid.NewGuid(), Name = "Dairy",  Slug = "dairy"  }
        };

        _repoMock.Setup(r => r.GetAllAsync(null, 1, 10))
            .ReturnsAsync(new PagedResultDto<Category>
            {
                Items      = fakeCategories,
                TotalCount = 2,
                Page       = 1,
                PageSize   = 10
            });

        var result = await _service.GetAllAsync(null, 1, 10);

        Assert.Equal(2, result.Items.Count);
        Assert.Equal("Fruits", result.Items[0].Name);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenSlugAlreadyExists()
    {
        var dto = new CreateCategoryDto { Name = "Fruits", Slug = "fruits" };

        _repoMock.Setup(r => r.ExistsBySlugAsync("fruits")).ReturnsAsync(true);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.CreateAsync(dto));

        Assert.Equal("Slug 'fruits' already exists.", ex.Message);
        _repoMock.Verify(r => r.CreateAsync(It.IsAny<Category>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenNotExists()
    {
        var id = Guid.NewGuid();
        _repoMock.Setup(r => r.DeleteAsync(id)).ReturnsAsync(false);

        var result = await _service.DeleteAsync(id);

        Assert.False(result);
    }
}