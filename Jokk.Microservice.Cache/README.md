# Cache
Setup Response caching, either In-Memory with the Concealed solution,\
or using Redis with the Distributed solution.

## AppSettings
<i>Properties are optional regarding the chosen solution</i>
```
"Cache" {
    "ConnectionString": "redis://localhost:6371"
}
```

## Startup
<i>Either add the Concealed or Distributed cache solution</i>
```
private IConfiguration Configuration { get; }
...
public void ConfigureServices(IServiceCollection services)
{
    //Concealed Caching
    app.AddMicroserviceMemoryCaching();
    
    //Distributed Caching
    app.AddMicroserviceDistributedCaching();
}
```