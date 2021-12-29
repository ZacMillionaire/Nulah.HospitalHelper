using Newtonsoft.Json;
using Nulah.HospitalHelper.Data;
using Nulah.HospitalHelper.Lib;
using Nulah.HospitalHelper.Api.Models;

namespace Nulah.HospitalHelper.Api
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = GetBuilder(args);
            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{builder.Environment.ApplicationName} v1");
                });
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
        static WebApplicationBuilder GetBuilder(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new() { Title = builder.Environment.ApplicationName, Version = "v1" });
                c.EnableAnnotations();
            });

            builder.Services.AddSingleton(ParseAppSettings());

            var dataRepository = new SqliteDataRepository($"Data Source=./hospitalhelper.db;");
            var bedRepository = new BedRepository(dataRepository);

            builder.Services.AddTransient(isp =>
            {
                return new BedManager(bedRepository);
            });

            return builder;

        }

        static CoreConfiguration ParseAppSettings()
        {
            try
            {
                var appsettingsConfig = $"{AppContext.BaseDirectory}/appsettings.json";
                if (File.Exists($"{AppContext.BaseDirectory}/appsettings.dev.json"))
                {
                    appsettingsConfig = $"{AppContext.BaseDirectory}/appsettings.dev.json";
                }

                using (FileStream fs = new FileStream(appsettingsConfig, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        var contents = sr.ReadToEnd();
                        return JsonConvert.DeserializeObject<CoreConfiguration>(contents) ?? new();
                    }
                }
            }
            catch
            {
                throw;
            }
        }
    }
}