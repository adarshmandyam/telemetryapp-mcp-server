using System.ComponentModel;
using System.Text;
using System.Text.Json;
using ModelContextProtocol.Server;

namespace CrudTelemetryApp.McpServer.Tools;

[McpServerToolType]
public partial class ProductApiTools
{
    private readonly HttpClient _httpClient;

    public ProductApiTools(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("ProductApi");
    }

    [McpServerTool, Description("Get all products from the API")]
    public async Task<string> GetProducts(
        [Description("Maximum number of products to return")] int? limit = null)
    {
        try
        {
            var url = limit.HasValue ? $"api/products?limit={limit}" : "api/products";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            return $"Error fetching products: {ex.Message}";
        }
    }

    [McpServerTool, Description("Get a product by ID")]
    public async Task<string> GetProductById(
        [Description("The product ID")] int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/products/{id}");
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return $"Product with ID {id} not found";

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            return $"Error fetching product: {ex.Message}";
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
            var product = new { Name = name, Description = description, Price = price };
            var content = new StringContent(
                JsonSerializer.Serialize(product),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync("api/products", content);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            return $"Error creating product: {ex.Message}";
        }
    }

    [McpServerTool, Description("Delete a product by ID")]
    public async Task<string> DeleteProduct(
        [Description("The product ID to delete")] int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/products/{id}");
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return $"Product with ID {id} not found";

            response.EnsureSuccessStatusCode();
            return $"Product {id} deleted successfully";
        }
        catch (Exception ex)
        {
            return $"Error deleting product: {ex.Message}";
        }
    }
}