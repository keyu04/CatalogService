using Asp.Versioning;
using CatalogService.Common.Constants;
using CatalogService.Common.Helpers;
using CatalogService.DTOs.Category;
using CatalogService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace CatalogService.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/categories")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _service;

    public CategoriesController(ICategoryService service) => _service = service;

    [HttpGet]
    [EnableRateLimiting("general")]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _service.GetAllAsync(search, page, pageSize);
        return Ok(ResponseHelper.Success<object>(result, LogConst.CATALOG_SERVICE + "_200"));
    }

    [HttpGet("{id:guid}")]
    [EnableRateLimiting("general")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var category = await _service.GetByIdAsync(id);
        if (category is null)
            return NotFound(ResponseHelper.Failure($"Category with id '{id}' not found."));

        return Ok(ResponseHelper.Success<object>(category, LogConst.CATALOG_SERVICE + "_200"));
    }

    [Authorize]
    [HttpPost]
    [EnableRateLimiting("strict")]
    public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return StatusCode(201, ResponseHelper.Success<object>(created, LogConst.CATALOG_SERVICE + "_201"));
    }

    [Authorize]
    [HttpPut("{id:guid}")]
    [EnableRateLimiting("strict")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryDto dto)
    {
        var updated = await _service.UpdateAsync(id, dto);
        if (updated is null)
            return NotFound(ResponseHelper.Failure($"Category with id '{id}' not found."));

        return Ok(ResponseHelper.Success<object>(updated, LogConst.CATALOG_SERVICE + "_200"));
    }

    [Authorize(Roles = "Admin")]  
    [HttpDelete("{id:guid}")]
    [EnableRateLimiting("strict")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted)
            return NotFound(ResponseHelper.Failure($"Category with id '{id}' not found."));

        return Ok(ResponseHelper.Success(LogConst.CATALOG_SERVICE + "_204"));
    }
}