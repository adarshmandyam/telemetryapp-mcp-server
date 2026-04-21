using System.Diagnostics;
using CrudTelemetryApp.Api.Services;
using CrudTelemetryApp.Api.Telemetry;
using CrudTelemetryApp.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace CrudTelemetryApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(IProductService productService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetAll()
    {
        var sw = Stopwatch.StartNew();
        var products = await productService.GetAllAsync();
        TelemetryConfig.RequestDuration.Record(sw.ElapsedMilliseconds, new KeyValuePair<string, object?>("operation", "GetAll"));
        return Ok(products);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Product>> GetById(int id)
    {
        var sw = Stopwatch.StartNew();
        var product = await productService.GetByIdAsync(id);
        TelemetryConfig.RequestDuration.Record(sw.ElapsedMilliseconds, new KeyValuePair<string, object?>("operation", "GetById"));
        return product is null ? NotFound() : Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<Product>> Create([FromBody] Product product)
    {
        var sw = Stopwatch.StartNew();
        var created = await productService.CreateAsync(product);
        TelemetryConfig.RequestDuration.Record(sw.ElapsedMilliseconds, new KeyValuePair<string, object?>("operation", "Create"));
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<Product>> Update(int id, [FromBody] Product product)
    {
        var sw = Stopwatch.StartNew();
        var updated = await productService.UpdateAsync(id, product);
        TelemetryConfig.RequestDuration.Record(sw.ElapsedMilliseconds, new KeyValuePair<string, object?>("operation", "Update"));
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var sw = Stopwatch.StartNew();
        var deleted = await productService.DeleteAsync(id);
        TelemetryConfig.RequestDuration.Record(sw.ElapsedMilliseconds, new KeyValuePair<string, object?>("operation", "Delete"));
        return deleted ? NoContent() : NotFound();
    }
}