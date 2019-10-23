using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using QuizApplication.ApiContracts.Interfaces;
using QuizApplication.Database;
using QuizApplication.Service;
using Serilog;

namespace QuizRestApi
{
    // TODO: Add Swagger to generate desired clients.
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(option => option.EnableEndpointRouting = false)
                    .SetCompatibilityVersion(CompatibilityVersion.Version_3_0).AddNewtonsoftJson(options =>
                     {
                         options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                         options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                         options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                     });

            InitializeDb(Configuration.GetConnectionString("QuizDb"));

            services.AddSingleton<IDbWithTransactionFactory, DbWithTransactionFactory>(
                provider => new DbWithTransactionFactory(Configuration.GetConnectionString("QuizDb")));
            services.AddSingleton<IDbFactory<QuizDbContext>, DbFactory<QuizDbContext>>(provider => new DbFactory<QuizDbContext>(Configuration.GetConnectionString("QuizDb")));
            services.AddSingleton<IQuizReadService, QuizReadService>();
            services.AddSingleton<IQuizWriteService, QuizWriteService>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment()) { app.UseDeveloperExceptionPage(); }

            app.UseSerilogRequestLogging();
            app.UseMvc();
        }

        private void InitializeDb(string connectionString)
        {
            if (System.IO.File.Exists(connectionString.Replace("DataSource=", "", StringComparison.InvariantCultureIgnoreCase))) { return; }

            var connection = new SqliteConnection(connectionString);
            connection.Open();
            // Migrate up
            var assembly = typeof(Startup).GetTypeInfo().Assembly;
            var migrationResourceNames = assembly.GetManifestResourceNames()
                                                 .Where(x => x.EndsWith(".sql"))
                                                 .OrderBy(x => x);
            if (!migrationResourceNames.Any()) { throw new System.Exception("No migration files found!"); }

            foreach (var resourceName in migrationResourceNames)
            {
                var sql = GetResourceText(assembly, resourceName);
                var command = connection.CreateCommand();
                command.CommandText = sql;
                command.ExecuteNonQuery();
            }

            connection.Close();
        }

        private static string GetResourceText(Assembly assembly, string resourceName)
        {
            using var stream = assembly.GetManifestResourceStream(resourceName);
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}
