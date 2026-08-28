using Database.Data;
using Microsoft.EntityFrameworkCore;

namespace Database
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' fehlt in appsettings.json.");
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString));

            // Der ESP32 und die WPF-App laufen auf anderen Rechnern im selben Netzwerk,
            // daher muss die API auf allen Netzwerkschnittstellen lauschen (siehe launchSettings.json/Kestrel).
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                });
            });

            var app = builder.Build();

            // Datenbank-Datei/Tabellen beim Start automatisch anlegen, falls noch nicht vorhanden.
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.EnsureCreated();
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors();

            // Kein HTTPS-Redirect: der ESP32 spricht die API nur per HTTP im lokalen Netzwerk an
            // und kann mit dem selbstsignierten Dev-Zertifikat nichts anfangen.

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
