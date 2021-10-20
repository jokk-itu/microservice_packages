# RateLimit

Setup RateLimiting for an API, either by using a concealed store with in-memory storage,\
or by using a distributed store with a redis database.

## Appsettings Code
```
    "RateLimit": {
        "ConnectionString": "redis://password@hostname:6379",
        "MinuteMax": 60,
        "HourMax": 80,
        "DayMax" 150
    }
```

## Startup Code
<i>Using the service should be one of the first services to be used.\
It is important that the RateLimit service,\
is registered as one of the first middlewares.
Therefore must also be called before UseRouting().</i>
```
private IConfiguration Configuration { get; }
...
public void ConfigureServices(IServiceCollection services)
{
    var configuration = Configuration.GetSection("RateLimit").Get<RateLimitConfiguration>();
    //Memory Store
    services.AddMicroserviceMemoryRateLimit(configuration);
    
    //Distributed Store
    services.AddMicroserviceDistributedRateLimit(configuration);
}
...
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    //Memory Store
    app.UseMicroserviceMemoryRateLimit();
    
    //Distributed Store
    app.UseMicroserviceDistributedRateLimit();
}
```
