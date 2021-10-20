# CORS

Implement a simple CORS configuration using two functions in Startup.
One could also setup allowed methods, but it is optional.

## Appsettings Code
```
"Cors": {
    "Services": ["http://localhost:5000", "http://localhost:5001"],
    "Methods": ["GET", "POST"]
}
```

## Startup Code
<i>There is no special order for the extension methods</i>
```
private IConfiguration Configuration { get; }
...
public void ConfigureServices(IServiceCollection services)
{
    var configuration = Configuration.GetSection("Cors").Get<CorsConfiguration>();
    services.AddMicroserviceCors(configuration);
}
...
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    app.UseMicroserviceCors();
}
```
