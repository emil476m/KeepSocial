using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.OpenApi.Models;
using Service;

namespace API;

public static class ServiceCollectionExtensions
{
    /*
     * adds JwtService when method is called
     */
    public static void AddJwtService(this IServiceCollection services)
    {
        services.AddSingleton<JwtOptions>(services =>
        {
            var configuration = services.GetRequiredService<IConfiguration>();
            var options = configuration.GetRequiredSection("JWT").Get<JwtOptions>()!;

            // If address isn't set in the config then we are likely running in development mode.
            // We will use the address of the server as *issuer* for JWT.
            if (string.IsNullOrEmpty(options?.Address))
            {
                var server = services.GetRequiredService<IServer>();
                var addresses = server.Features.Get<IServerAddressesFeature>()?.Addresses;
                options.Address = addresses?.FirstOrDefault();
            }

            return options;
        });
        services.AddSingleton<JwtService>();
    }
    
    /*
     * adds SwaggerGen with BearerJwt when method is called
     */
    public static void AddSwaggerGenWithBearerJWT(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please insert JWT with Bearer into field",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                        new string[] { }
                    }
                });
            }
        );
    }
    
    public static void AddAvatarBlobService(this IServiceCollection services)
    {
        services.AddSingleton<BlobService>(provider =>
        {
            // Get connection string from configuration (appsettings.json)
            string connectionString = Environment.GetEnvironmentVariable("AvatarStorage");
                //.GetConnectionString("AvatarStorage"); //TODO Envirement variable // Secret
            // The client knows how to talk to the service on Azure.
            var client = new BlobServiceClient(connectionString);
            // Return an instance of the service we just made.
            return new BlobService(client);
        });
    }
}