using Microsoft.AspNetCore.Authentication.Cookies;
using Nulah.HospitalHelper.Api.Controllers;
using Nulah.HospitalHelper.Data;
using Nulah.HospitalHelper.Lib;

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
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }

        static WebApplicationBuilder GetBuilder(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddMvc();

#if DEBUG
            ConfigureDependencies(builder.Services);
#endif

            builder.Services.AddControllers();
            //builder.Services.AddSingleton(ParseAppSettings());

            return builder;
        }

#if DEBUG
        /// <summary>
        /// Used only for debug mode, and creates a hard dependency on the API project
        /// </summary>
        /// <param name="services"></param>
        static void ConfigureDependencies(IServiceCollection services)
        {
            // Create one off instances of repositories required
            var dataRepository = new SqliteDataRepository($"Data Source=./hospitalhelper.db;");

            dataRepository.InitialiseDatabase();

            var bedRepository = new BedRepository(dataRepository);
            var patientRepository = new PatientRepository(dataRepository);
            var userRepository = new UserRepository(dataRepository);
            var employeeRepository = new EmployeeRepository(dataRepository);

            var bedManager = new BedManager(bedRepository, patientRepository);
            var userManager = new UserManager(userRepository);
            var employeeManager = new EmployeeManager(employeeRepository);
            var patientManager = new PatientManager(patientRepository, bedRepository);

            // Define transient (per request) API classes to be injected
            // This is only done in debug as a release version should not depend on the API project
            // but instead use a front end framework to create a decoupled relationship
            services.AddTransient(isp => new BedApiController(bedManager));
            services.AddTransient(isp => new UserApiController(userManager));
            services.AddTransient(isp => new EmployeeApiController(employeeManager));
            services.AddTransient(isp => new PatientApiController(patientManager, employeeManager, bedManager));

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(x =>
                {
                    x.LoginPath = "/Login";
                    x.LogoutPath = "/Logout";
                });
        }

#endif

        /*
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
        */
    }
}