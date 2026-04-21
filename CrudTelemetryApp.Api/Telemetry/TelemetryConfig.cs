using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace CrudTelemetryApp.Api.Telemetry;

public static class TelemetryConfig
{
    public const string ServiceName = "CrudTelemetryApp";
    public const string ServiceVersion = "1.0.0";

    public static readonly ActivitySource ActivitySource = new(ServiceName, ServiceVersion);
    public static readonly Meter Meter = new(ServiceName, ServiceVersion);

    // Custom metrics
    public static readonly Counter<long> ProductsCreated = Meter.CreateCounter<long>(
        "products_created_total", "products", "Total number of products created");

    public static readonly Counter<long> ProductsDeleted = Meter.CreateCounter<long>(
        "products_deleted_total", "products", "Total number of products deleted");

    public static readonly Histogram<double> RequestDuration = Meter.CreateHistogram<double>(
        "request_duration_ms", "ms", "Request duration in milliseconds");

    public static readonly UpDownCounter<int> ActiveProducts = Meter.CreateUpDownCounter<int>(
        "active_products", "products", "Current number of active products");
}