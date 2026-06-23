using Asp.Versioning;
using CatalogService.Common.Constants;
using CatalogService.Common.Helpers;
using CatalogService.DTOs.Product;
using CatalogService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace CatalogService.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/products")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _service;

    public ProductsController(IProductService service) => _service = service;

    [HttpGet]
    [EnableRateLimiting("general")]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search,
        [FromQuery] Guid? categoryId,
        [FromQuery] bool? isFeatured,
        [FromQuery] bool? inStockOnly,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _service.GetAllAsync(
            search, categoryId, isFeatured, inStockOnly, page, pageSize);

        return Ok(ResponseHelper.Success<object>(result, LogConst.PRODUCT_SERVICE + "_200"));
    }

    [HttpGet("{id:guid}")]
    [EnableRateLimiting("general")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var product = await _service.GetByIdAsync(id);
        if (product is null)
            return NotFound(ResponseHelper.Failure($"Product with id '{id}' not found."));

        return Ok(ResponseHelper.Success<object>(product, LogConst.PRODUCT_SERVICE + "_200"));
    }

    [Authorize]
    [HttpPost]
    [EnableRateLimiting("strict")]
    public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return StatusCode(201, ResponseHelper.Success<object>(created, LogConst.PRODUCT_SERVICE + "_201"));
    }

    [Authorize]
    [HttpPut("{id:guid}")]
    [EnableRateLimiting("strict")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductDto dto)
    {
        var updated = await _service.UpdateAsync(id, dto);
        if (updated is null)
            return NotFound(ResponseHelper.Failure($"Product with id '{id}' not found."));

        return Ok(ResponseHelper.Success<object>(updated, LogConst.PRODUCT_SERVICE + "_200"));
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:guid}")]
    [EnableRateLimiting("strict")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted)
            return NotFound(ResponseHelper.Failure($"Product with id '{id}' not found."));

        return Ok(ResponseHelper.Success(LogConst.PRODUCT_SERVICE + "_204"));
    }
}