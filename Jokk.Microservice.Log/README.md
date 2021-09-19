# Log

Setup Serilog for an API.\
The available sinks are Seq, Console and UDP.\
The package also keeps track of a CorrelationId,\
which is received by a predefined header and send along all HTTP requests.\
The header is: X-Correlation-Id'.

## AppSettings Code
<i>The following is optional, except 'Service'.</i>
```
"Logging": {
    "LogToConsole": true,
    "LogToUDP": true,
    "LogToElastic": true,
    "LogToSeq": true,
    "SeqUrl": "http://localhost:5341",
    "ElasticUrl": "http://localhost:6000",
    "UDPHost: "localhost",
    "UDPPort: "3000",
    "Overrides" {
        "Microsoft": "Warning",
        "Microsoft.Hosting.Lifetime": "Error"
    },
    "Service": "NameofService"
}
```

## Startup Code
<i>There is no call order.</i>
```
private IConfiguration Configuration { get; }
...
public void ConfigureServices(IServiceCollection services)
{
    services.AddMicroserviceLogging(Configuration.GetSection("Logging"));
}
...
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    services.UseMicroserviceLogging();
}
```

## Program Code
```
public static void Main(string[] args) 
{
    Host
        .CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder => 
        {
            webBuilder.UseStartup<Startup>()
        })
        .AddMicroserviceLogging()
        .Build
        .Run();
}
```