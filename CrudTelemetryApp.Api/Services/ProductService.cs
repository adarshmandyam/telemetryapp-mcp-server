using System.Diagnostics;
using CrudTelemetryApp.Api.Data;
using CrudTelemetryApp.Api.Telemetry;
using CrudTelemetryApp.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace CrudTelemetryApp.Api.Services;

public interface IProductService
{
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task<Product> CreateAsync(Product product);
    Task<Product?> UpdateAsync(int id, Product product);
    Task<bool> DeleteAsync(int id);
}

public class ProductService : IProductService
{
    private readonly ProductDbContext _context;

    public ProductService(ProductDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        using var activity = TelemetryConfig.ActivitySource.StartActivity("GetAllProducts");

        var products = await _context.Products.ToListAsync();
        activity?.SetTag("product.count", products.Count);

        return products;
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        using var activity = TelemetryConfig.ActivitySource.StartActivity("GetProductById");
        activity?.SetTag("product.id", id);

        var product = await _context.Products.FindAsync(id);
        activity?.SetTag("product.found", product is not null);

        return product;
    }

    public async Task<Product> CreateAsync(Product product)
    {
        using var activity = TelemetryConfig.ActivitySource.StartActivity("CreateProduct");

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        activity?.SetTag("product.id", product.Id);
        activity?.SetTag("product.name", product.Name);

        TelemetryConfig.ProductsCreated.Add(1);
        TelemetryConfig.ActiveProducts.Add(1);

        return product;
    }

    public async Task<Product?> UpdateAsync(int id, Product product)
    {
        using var activity = TelemetryConfig.ActivitySource.StartActivity("UpdateProduct");
        activity?.SetTag("product.id", id);

        var existing = await _context.Products.FindAsync(id);
        if (existing is null)
        {
            activity?.SetStatus(ActivityStatusCode.Error, "Product not found");
            return null;
        }

        existing.Name = product.Name;
        existing.Description = product.Description;
        existing.Price = product.Price;

        await _context.SaveChangesAsync();

        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var activity = TelemetryConfig.ActivitySource.StartActivity("DeleteProduct");
        activity?.SetTag("product.id", id);

        var product = await _context.Products.FindAsync(id);
        if (product is null)
        {
            activity?.SetStatus(ActivityStatusCode.Error, "Product not found");
            return false;
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        TelemetryConfig.ProductsDeleted.Add(1);
        TelemetryConfig.ActiveProducts.Add(-1);

        return true;
    }
}