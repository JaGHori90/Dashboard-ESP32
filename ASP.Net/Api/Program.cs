
using Core.Contracts;
using Microsoft.OpenApi.Models;
using Persistence;

namespace WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers()
                // Explizit angeben statt Assembly.GetEntryAssembly() zu vertrauen:
                // unter WebApplicationFactory in Tests ist die "Entry Assembly" der
                // Testrunner, nicht WebApi - ohne das hier findet AddControllers()
                // dann keine einzige Route und alles liefert 404.
                .AddApplicationPart(typeof(Program).Assembly)
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
                });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

            builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                // "Authorize"-Button in Swagger UI: Key einmal eintragen, wird danach bei
                // "Try it out" automatisch als X-Api-Key-Header mitgeschickt (z.B. für Post/
                // Cleanup/UpdateSensorById).
                options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
                {
                    Name = "X-Api-Key",
                    Type = SecuritySchemeType.ApiKey,
                    In = ParameterLocation.Header,
                    Description = "API-Key für schreibende Endpunkte (Post, Cleanup, UpdateSensorById)."
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "ApiKey" }
                        },
                        Array.Empty<string>()
                    }
                });
            });



            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
