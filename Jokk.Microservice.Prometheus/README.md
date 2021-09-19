# Prometheus

Setup Prometheus for monitoring to a prometheus instance.\
Also sets up healthchecks, which will be monitored.\
As well as outgoing HTTP requests and system metrics.

## Appsettings Code
<i>The following settings are optional.\
Services are a collection URI's which will be health checked,\
on the following relative path: '/health'.\
SQLServer, Mongo, Neo4j and Redis are also used for healthchecks.
</i>
```
"Prometheus": {
    "Services": {
        "Servicename": "http://localhost:5000",
        "Servicename2": "http://localhost:5001"
    },
    "SqlServerConnectionString": "Server.;User Id=sa;Password=password",
    "MongoUri": "mongo://localhost:4000",
    "MongoUsername": "username",
    "MongoPassword": "password",
    "Neo4jUri": "neo4j://localhost:3000",
    "Neo4jUsername": "username",
    "Neo4jPassword": "password",
    "RedisConnectionString: "redis://localhost:6379",
    "RedisDatabase": "database"
}
```

## Startup Code
<i>The following code does not need a call order</i>
```
private IConfiguration Configuration { get; }
...
public void ConfigureServices(IServiceCollection services)
{
    services.AddMicroservicePrometheus(IConfiguration.GetSection("Prometheus"));
}
...
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    app.UseMicroservicePrometheus();
}
```