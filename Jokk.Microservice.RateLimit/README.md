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
is registered as one of the first middlewares.</i>
```
private IConfiguration Configuration { get; }
...
public void ConfigureServices(IServiceCollection services)
{
    //Concealed Store
    services.AddMicroserviceConcealedRateLimit();
    
    //Distributed Store
    services.AddMicroserviceDistributedRateLimit();
}
...
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    //Concealed Store
    app.UseMicroserviceConcealedRateLimit();
    
    //Distributed Store
    app.UseMicroserviceDistributedRateLimit();
}
```
