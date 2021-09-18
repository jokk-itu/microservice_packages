# Swagger

Setup Swagger support, by using Bearer tokens as authorization,\
or anonymously with no authorization.\
There is no support for multiple documents/versions.

## Startup Code
<i>The order of function calls, does not matter.</i>
```
public void ConfigureServices(IServiceCollection services)
{
    //Anonymous
    services.AddSwaggerAnonymous();
    
    //Bearer Authorization
    services.AddSwaggerAuthorization();
}
...
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    app.UseMicroserviceSwagger();
}
```
