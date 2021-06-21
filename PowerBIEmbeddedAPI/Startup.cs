using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using PowerBIEmbeddedAPI.Models;
using PowerBIEmbeddedAPI.Services;

namespace PowerBIEmbeddedAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            // Registrar AadService y PbiEmbedService para la inyección de dependencias
            services.AddScoped(typeof(AadService))
                    .AddScoped(typeof(PbiEmbedService));

            services.AddControllersWithViews();

            // Cargando appsettings.json en clases de modelo C #
            services.Configure<AzureAd>(Configuration.GetSection("AzureAd"))
                    .Configure<PowerBI>(Configuration.GetSection("PowerBI"));
            
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PowerBIEmbeddedAPI", Version = "v1" });
            });
        }

    
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PowerBIEmbeddedAPI v1"));
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            //CORS!!!!!
            app.UseCors(opt =>
            {
                opt.AllowAnyHeader();
                opt.AllowAnyMethod();
                opt.AllowAnyOrigin();

            });
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
