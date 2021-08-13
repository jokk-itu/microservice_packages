﻿using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Jokk.Microservice.Swagger
{
    public static class Setup
    {
        public static IServiceCollection AddSwaggerAnonymous(this IServiceCollection services)
        {
            return services.AddSwaggerGen(c => c.SwaggerDoc("v1", new OpenApiInfo()
            {
                Title = "WebApi",
                Version = "v1"
            }));
        }
        
        public static IServiceCollection AddSwaggerAuthorization(this IServiceCollection services)
        {
            return services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "WebApi", Version = "v1"});
                var securitySchema = new OpenApiSecurityScheme
                {
                    Description =
                        "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                };
                c.AddSecurityDefinition("Bearer", securitySchema);
                var securityRequirement = new OpenApiSecurityRequirement
                {
                    {securitySchema, new[] {"Bearer"}}
                };
                c.AddSecurityRequirement(securityRequirement);
            });
        }

        public static IApplicationBuilder UseMicroserviceSwagger(this IApplicationBuilder app)
        {
            return app.UseSwagger();
        }
    }
}