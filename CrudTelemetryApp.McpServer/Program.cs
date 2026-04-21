using CrudTelemetryApp.McpServer;
using CrudTelemetryApp.McpServer.Tools;

var builder = WebApplication.CreateBuilder(args);

// Configure HttpClient factory
builder.Services.AddHttpClient("TelemetryApi", client =>
{
    var telemetryBase =
        builder.Configuration["Telemetry:TracesEndpoint"] ??
        builder.Configuration["Telemetry:QueryEndpoint"] ??
        "http://localhost:16686";

    client.BaseAddress = new Uri(telemetryBase);
});
builder.Services.AddHttpClient("ProductApi", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Api:BaseUrl"]!);
});

// Configure MCP Server
builder.Services
    .AddMcpServer(options =>
    {
        options.ServerInfo = new()
        {
            Name = "CrudTelemetryApp MCP Server",
            Version = "1.0.0"
        };
    })
    .WithHttpTransport(options =>
    {
        options.Stateless = true;  // Enable stateless mode for HTTP clients
    })
    .WithToolsFromAssembly();

var app = builder.Build();

// Map MCP endpoint with explicit route
app.MapMcp("/mcp");

app.Run();
