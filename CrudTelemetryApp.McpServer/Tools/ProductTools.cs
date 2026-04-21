using System.ComponentModel;
using System.Text.Json;
using CrudTelemetryApp.Shared.Models;
using ModelContextProtocol.Server;
using Microsoft.Extensions.Logging;

namespace CrudTelemetryApp.McpServer.Tools;

[McpServerToolType]
public partial class ProductTools
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ProductTools> _logger;

    public ProductTools(IHttpClientFactory httpClientFactory, ILogger<ProductTools> logger)
    {
        _logger = logger;
        try
        {
            _logger.LogInformation("Constructing ProductTools");
            _httpClient = httpClientFactory.CreateClient("ProductApi");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ProductTools constructor failed");
            throw;
        }
    }

    [McpServerTool, Description("Get all products from the API")]
    public async Task<string> GetAllProducts()
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/products");
            if (response.IsSuccessStatusCode)
            {
                var products = await response.Content.ReadFromJsonAsync<List<Product>>();
                return JsonSerializer.Serialize(products, new JsonSerializerOptions { WriteIndented = true });
            }
            return $"Failed to get products: {response.StatusCode}";
        }
        catch (Exception ex)
        {
            return $"Error getting products: {ex.Message}";
        }
    }

    [McpServerTool, Description("Get a product by its ID")]
    public async Task<string> GetProductById(
        [Description("The product ID to retrieve")] int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/products/{id}");
            if (response.IsSuccessStatusCode)
            {
                var product = await response.Content.ReadFromJsonAsync<Product>();
                return JsonSerializer.Serialize(product, new JsonSerializerOptions { WriteIndented = true });
            }
            return response.StatusCode == System.Net.HttpStatusCode.NotFound 
                ? $"Product with ID {id} not found" 
                : $"Failed to get product: {response.StatusCode}";
        }
        catch (Exception ex)
        {
            return $"Error getting product: {ex.Message}";
        }
    }

    [McpServerTool, Description("Create a new product")]
    public async Task<string> CreateProduct(
        [Description("Product name")] string name,
        [Description("Product description")] string? description,
        [Description("Product price")] decimal price)
    {
        try
        {
            var product = new Product { Name = name, Description = description, Price = price };
            var response = await _httpClient.PostAsJsonAsync("/api/products", product);
            
            if (response.IsSuccessStatusCode)
            {
                var created = await response.Content.ReadFromJsonAsync<Product>();
                return JsonSerializer.Serialize(created, new JsonSerializerOptions { WriteIndented = true });
            }
            return $"Failed to create product: {response.StatusCode}";
        }
        catch (Exception ex)
        {
            return $"Error creating product: {ex.Message}";
        }
    }
}