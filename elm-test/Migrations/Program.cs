using DbUp;
using DbUp.Engine;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace Migrations
{
    internal class Program
    {
        private static Microsoft.Extensions.Configuration.IConfiguration? _configuration;
        public static Microsoft.Extensions.Configuration.IConfiguration AppConfiguration
        {
            get { return _configuration ?? throw new Exception("Configuration is not initialized yet"); }
            private set { _configuration = value; }
        }

        static int Main(string[] args)
        {
            AppConfiguration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();


            var upgrader = BuildUpgrader(AppConfiguration);

            var connectionString = AppConfiguration.GetConnectionString("DefaultConnection");
            EnsureDatabase.For.SqlDatabase(connectionString);

            if (!CheckIfDbUserShouldBeCreated(AppConfiguration, upgrader))
                upgrader = BuildUpgrader(AppConfiguration, false);



            var result = upgrader.PerformUpgrade();

            if (!result.Successful)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(result.Error);
                Console.ResetColor();
#if DEBUG
                Console.ReadLine();
#endif
                return -1;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Success!");

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddCors();
            builder.Services.AddControllers();
            builder.Services.AddScoped<IDapperContextDb, DapperContextDb>();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

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

            app.UseCors(
                options => options.WithOrigins("http://localhost:4200").AllowAnyMethod()
            );
            app.Run();

            Console.ResetColor();
            return 0;
        }

        private static UpgradeEngine BuildUpgrader(IConfiguration config, bool createDbUserIfNotExist = true)
        {
            var connectionString = config.GetConnectionString("DefaultConnection");
            var database = ExtractDbNameFromConnectionString(connectionString ?? throw new Exception("Error finding DefaultConnection in appsettings"));

            var varList = config.GetSection("ScriptVariables")
                .AsEnumerable()
                .Where(x => x.Key.StartsWith("ScriptVariables:"))
                .ToDictionary(kvp => kvp.Key.Substring("ScriptVariables:".Length), kvp => kvp.Value);

            return DeployChanges.To
                .SqlDatabase(connectionString)
                .WithVariable("DatabaseName", database)
                .WithVariable("ShouldCreateDbUser", createDbUserIfNotExist ? "1" : "0")
                .WithVariables(varList)
                .WithScriptsAndCodeEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                .WithTransactionPerScript()
                .WithExecutionTimeout(TimeSpan.FromMinutes(5))
                .LogToConsole()
                .WithPreprocessor(new DummyScriptPreprocessor())
                .Build();
        }

        private static string ExtractDbNameFromConnectionString(string connectionString)
        {
            var conn = new SqlConnectionStringBuilder(connectionString);
            return conn.InitialCatalog;
        }

        private static bool CheckIfDbUserShouldBeCreated(IConfiguration config, UpgradeEngine upgrader)
        {
            var dbUser = config.GetValue<string>("ScriptVariables:DbUserName");
            var executedScripts = upgrader.GetExecutedScripts();
            if (executedScripts.Count == 0)
            {
                return CollectInputBoolean($"Do you want to create SQL Database User '{(dbUser)}'?");
            }
            return false;
        }

        private static bool CollectInputBoolean(string message)
        {
            Console.WriteLine("------------------------------------------");
            Console.WriteLine(message);
            Console.WriteLine("[y]es, [n]o? ");
            while (true)
            {
                var answer = Console.ReadLine();
                if (!string.IsNullOrEmpty(answer))
                {
                    answer = answer.ToLower();
                    if (answer == "yes" || answer == "y")
                        return true;
                    if (answer == "no" || answer == "n")
                        return false;
                }
            }
        }

        public static IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder().ConfigureWebHostDefaults(webHost => {
                webHost.UseStartup<Startup>();
            });
        }
    }
}