using Newtonsoft.Json;
using Nulah.HospitalHelper.Data;
using Nulah.HospitalHelper.Lib;
using Nulah.HospitalHelper.Api.Models;
using Nulah.HospitalHelper.Api.Controllers;
using Microsoft.OpenApi.Models;

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

            // Create one off instances of repositories required
            var dataRepository = new SqliteDataRepository($"Data Source=./hospitalhelper.db;");

#if DEBUG
            dataRepository.InitialiseDatabase();
#endif

            var bedRepository = new BedRepository(dataRepository);
            var patientRepository = new PatientRepository(dataRepository);
            var userRepository = new UserRepository(dataRepository);
            var employeeRepository = new EmployeeRepository(dataRepository);

            // Define transient (per request) manager classes to be injected
            builder.Services.AddTransient(isp => new BedManager(bedRepository, patientRepository));
            builder.Services.AddTransient(isp => new UserManager(userRepository));
            builder.Services.AddTransient(isp => new EmployeeManager(employeeRepository));
            builder.Services.AddTransient(isp => new PatientManager(patientRepository, bedRepository));

            // Add lazy api authentication
            // And it is _very_ lazy
            builder.Services
                .AddAuthentication(opts => opts.DefaultScheme = LazyApiAuthentication.AuthenticationSchemes)
                .AddScheme<LazyApiSchemeOptions, LazyApiAuthentication>(LazyApiAuthentication.AuthenticationSchemes, null);

            builder.Services.AddControllers();

            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new() { Title = builder.Environment.ApplicationName, Version = "v1" });
                c.EnableAnnotations();

                // Add definition for a bearer token to enable swagger to authorise
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Authorisation header using an API Key",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer"
                });

                // Add requirement for a bearer token to enable swagger to authorise
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new List<string>()
                    }
                });
            });

            builder.Services.AddSingleton(ParseAppSettings());

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