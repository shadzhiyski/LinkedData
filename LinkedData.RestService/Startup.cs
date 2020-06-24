using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphiQl;
using GraphQL;
using LinkedData.Data.External;
using LinkedData.Data.Repositories;
using LinkedData.RestService.Models.GraphQL;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LinkedData.RestService
{
    public class Startup
    {
        public const int Port = 5000;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false;
                options.SslPort = Port;
                options.Filters.Add(new RequireHttpsAttribute());
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddAntiforgery(
                options =>
                {
                    options.Cookie.Name = "_af";
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.HeaderName = "X-XSRF-TOKEN";
                }
            );

            services.AddSingleton<IDocumentExecuter, DocumentExecuter>();

            services.AddSingleton<Neo4jDataService>();
            services.AddSingleton<EbiDataService>();
            services.AddSingleton<EnsemblDataService>();

            services.AddSingleton<GenesRepository>();
            services.AddSingleton<ProteinsRepository>();

            services.AddSingleton<LinkedDataQuery>();
            services.AddSingleton<LinkedDataMutation>();
            services.AddTransient<CommentType>();
            services.AddTransient<GeneType>();
            services.AddTransient<VariationType>();
            services.AddTransient<GeneWithRelationsType>();
            services.AddTransient<ProteinType>();
            services.AddTransient<ProteinWithRelationsType>();
            services.AddTransient<GeneInputType>();
            var sp = services.BuildServiceProvider();
            services.AddSingleton<LinkedDataSchema>(new LinkedDataSchema(new FuncDependencyResolver(type => sp.GetService(type))));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseGraphiQl("/graphiql", "/graphql");
            app.UseHttpsRedirection();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}