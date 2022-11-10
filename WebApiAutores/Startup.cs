using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Text.Json.Serialization;
using WebApiAutores.Middlewares;
using WebApiAutores.Servicios;

namespace WebApiAutores
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Add services to the container.
            services.AddControllers().AddJsonOptions(
                x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles
                );

            services.AddDbContext<ApplicationDbContext>(
                options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"))
                );

            //Inyección de dependencias ini
            services.AddTransient<IServicio,ServicioA>();

            services.AddTransient<ServicioTransient>();
            services.AddScoped<ServicioScoped>();
            services.AddSingleton<ServicioSingleton>();
            //Inyección de dependencias fin

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
        }

        public void Configure(IApplicationBuilder application, IWebHostEnvironment hostEnvironment, ILogger<Startup> logger)
        {

            //Prueba de middleware ini
            //application.UseMiddleware<LoguearRespuestaHTTPMiddleware>();
            application.UseLoguearRespuestaHTTP();

            application.Map("/rutaUno", application =>
            {
                application.Run(async contexto =>
                {
                    await contexto.Response.WriteAsync("Se intercepta la tubería");
                });
            });
            //Prueba de middleware fin

            // Configure the HTTP request pipeline.
            if (hostEnvironment.IsDevelopment())
            {
                application.UseSwagger();
                application.UseSwaggerUI();
            }

            application.UseHttpsRedirection();
            application.UseRouting();
            application.UseAuthorization();

            application.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
