# CORS

Implement a simple CORS configuration using two functions in Startup.
One could also setup allowed methods, but it is optional.

## Appsettings Code
Where 'Servicename' could be any name
```
"Services": {
    "Servicename": "http://localhost:5000",
    "Servicename2": "http://localhost:5001"
},
"Methods": ["GET", "POST"]
```

## Startup Code
<i>There is no special order for the extension methods</i>
```
private IConfiguration Configuration { get; }
...
public void ConfigureServices(IServiceCollection services)
{
    services.AddMicroserviceCors(
        Configuration.GetSection("Services"), 
        Configuration.GetSection("Methods"));
}
...
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    app.UseMicroserviceCors();
}
```
