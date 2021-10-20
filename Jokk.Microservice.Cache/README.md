# Cache
Setup Response caching, either In-Memory with the Concealed solution,\
or using Redis with the Distributed solution.

## AppSettings
<i>Properties are optional regarding the chosen solution</i>
```
"Cache" {
    "Host": "localhost",
    "Port": "6371",
    "Password": "password"
}
```

## Startup
<i>Either add the Concealed or Distributed cache solution</i>
```
private IConfiguration Configuration { get; }
...
public void ConfigureServices(IServiceCollection services)
{
    var configuration = Configuration.GetSection("Cache").Get<CacheConfiguration>();
    
    //Memory Caching
    app.AddMicroserviceMemoryCaching();
    
    //Distributed Caching
    app.AddMicroserviceDistributedCaching(configuration);
    
    //Client Caching
    app.AddMicroserviceClientCache();
}

public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    ...
    app.UseMicroserviceClientCache();
    ...
}
```