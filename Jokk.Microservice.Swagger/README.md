# RateLimit

Setup Ratelimiting for an API, either by using a concealed store with in-memory storage,\
or by using a distributed store with a redis database.

## Appsettings Code
```

```

## Startup Code
```
private IConfiguration Configuration { get; }
...
public void ConfigureServices(IServiceCollection services)
{
    
}
...
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    
}
```
