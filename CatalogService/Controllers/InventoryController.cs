using Asp.Versioning;
using CatalogService.Common.Constants;
using CatalogService.Common.Helpers;
using CatalogService.DTOs.Inventory;
using CatalogService.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace CatalogService.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/inventory")]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _service;

    public InventoryController(IInventoryService service) => _service = service;

    [HttpGet("{productId:guid}")]
    [EnableRateLimiting("general")]
    public async Task<IActionResult> GetByProductId(Guid productId)
    {
        var inventory = await _service.GetByProductIdAsync(productId);
        if (inventory is null)
            return NotFound(ResponseHelper.Failure($"Inventory for product '{productId}' not found."));

        return Ok(ResponseHelper.Success<object>(inventory, LogConst.INVENTORY_SERVICE + "_200"));
    }

    [HttpPut("{productId:guid}")]
    [EnableRateLimiting("strict")]
    public async Task<IActionResult> Update(Guid productId, [FromBody] UpdateInventoryDto dto)
    {
        var updated = await _service.UpdateAsync(productId, dto);
        if (updated is null)
            return NotFound(ResponseHelper.Failure($"Inventory for product '{productId}' not found."));

        return Ok(ResponseHelper.Success<object>(updated, LogConst.INVENTORY_SERVICE + "_200"));
    }

    [HttpPost("{productId:guid}/reserve")]
    [EnableRateLimiting("strict")]
    public async Task<IActionResult> Reserve(Guid productId, [FromQuery] int quantity)
    {
        if (quantity <= 0)
            return BadRequest(ResponseHelper.Failure("Quantity must be greater than 0."));

        var reserved = await _service.ReserveStockAsync(productId, quantity);
        if (!reserved)
            return BadRequest(ResponseHelper.Failure("Insufficient stock to reserve."));

        return Ok(ResponseHelper.Success(LogConst.INVENTORY_SERVICE + "_200"));
    }

    [HttpPost("{productId:guid}/release")]
    [EnableRateLimiting("strict")]
    public async Task<IActionResult> Release(Guid productId, [FromQuery] int quantity)
    {
        if (quantity <= 0)
            return BadRequest(ResponseHelper.Failure("Quantity must be greater than 0."));

        var released = await _service.ReleaseStockAsync(productId, quantity);
        if (!released)
            return NotFound(ResponseHelper.Failure($"Inventory for product '{productId}' not found."));

        return Ok(ResponseHelper.Success(LogConst.INVENTORY_SERVICE + "_200"));
    }
}