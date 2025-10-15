using System.Text.Json;
using Atleta.Core;
using Atleta.Core.Interface;
using Atleta.Core.Interface.API;
using Atleta.Core.Services;
using Atleta.Core.Services.Background;
using Atleta.Infrastructure.Cache;
using Atleta.Infrastructure.Data;
using Atleta.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Refit;

namespace Atleta.Api
{
    public class Startup(IConfiguration configuration)
    {
        public IConfiguration Configuration { get; } = configuration;

        // Aqui configuramos os serviços
        public void ConfigureServices(IServiceCollection services)
        {
            // Controllers
            services.AddControllers();

            // Swagger
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Atleta API",
                    Version = "v1",
                    Description = "API para gerenciamento de Atletas no sistema"
                });
            });

            services.AddDbContext<AtletaDbContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"),
                    o => o.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null)));

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddScoped<IAtletaHandler, AtletaHandler>();
            services.AddScoped<IAtletaRepository, AtletaRepository>();
            services.AddScoped<IAtletaService, AtletaService>();
            services.AddScoped<ICacheService, CacheService>();
            services.AddScoped<IMensageriaService, MensageriaService>();
            services.AddHostedService<WorkerAtualizaCacheHostedService>();

            services.AddRefitClient<IUsuarioService>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri("http://localhost:5249"));

            services.AddRefitClient<IGinasioService>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri("http://localhost:5183"));
                
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = Configuration.GetConnectionString("Redis");
                options.InstanceName = "AtletaApp_";
            });
        
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    policy => policy
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Atleta API v1");
                    c.RoutePrefix = string.Empty;
                });
            }

            app.UseRouting();

            app.UseCors("AllowAll");

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}