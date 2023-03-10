using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Text.Json.Serialization;
using WebApiAutores.Middlewares;
using WebApiAutores.Filtros;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using WebApiAutores.Servicios;
using WebApiAutores.Utilidades;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;

[assembly: ApiConventionType(typeof(DefaultApiConventions))]
namespace WebApiAutores
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Add services to the container.
            services.AddControllers(opciones =>
            {
                opciones.Filters.Add(typeof(FiltroDeExcepcion));
                opciones.Conventions.Add(new SwaggerAgrupaPorVersion());
            }).AddJsonOptions(
                x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles
                ).AddNewtonsoftJson();

            services.AddDbContext<ApplicationDbContext>(
                options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"))
                );

            //Autenticación ini
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(ops => ops.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer= false,
                    ValidateAudience= false,
                    ValidateLifetime=true,
                    ValidateIssuerSigningKey=true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(Configuration["llaveJwt"])
                        ),
                    ClockSkew=TimeSpan.Zero
                });
            //Autenticación fin
            
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(
                    "v1", 
                    new OpenApiInfo { 
                        Title = "WebApiAutores", 
                        Version = "v1",
                        Description = "Web Api de Autores y Libros",
                        Contact = new OpenApiContact
                        {
                            Email = "zutjmx@gmail.com",
                            Name = "Jesús Zúñiga Trejo",
                            Url = new Uri("https://github.com/zutjmx")
                        }, 
                        License = new OpenApiLicense
                        {
                            Name= "MIT",
                        }
                    }
                );
                c.SwaggerDoc("v2", new OpenApiInfo { Title = "WebApiAutores", Version = "v2"});

                c.OperationFilter<AgregarParametroHATEOAS>();
                c.OperationFilter<AgregarParametroXVersion>();

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name="Authorization",
                    Type=SecuritySchemeType.ApiKey,
                    Scheme= "Bearer",
                    BearerFormat="JWT",
                    In=ParameterLocation.Header
                });

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
                        new string[]{}
                    }
                });

                var archivoXml = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var rutaXml = Path.Combine(AppContext.BaseDirectory,archivoXml);
                c.IncludeXmlComments(rutaXml);

            });

            services.AddAutoMapper(typeof(Startup));

            services.AddIdentity<IdentityUser,IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthorization(opciones =>
            {
                opciones.AddPolicy("EsAdmin", politica => politica.RequireClaim("esAdmin"));
            });

            services.AddDataProtection();
            services.AddTransient<HashService>();

            services.AddCors(opciones =>
            {
                opciones.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .WithExposedHeaders(new string[] { "cantidadTotalRegistros" });
                });
            });

            services.AddTransient<GeneradorEnlaces>();
            services.AddTransient<HATEOASAutorFiltroAttribute>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services.AddScoped<ServicioLlaves>();

        }

        public void Configure(IApplicationBuilder application, IWebHostEnvironment hostEnvironment, ILogger<Startup> logger)
        {

            //Prueba de middleware ini
            //application.UseMiddleware<LoguearRespuestaHTTPMiddleware>();
            application.UseLoguearRespuestaHTTP();

            //Prueba de middleware fin

            // Configure the HTTP request pipeline.
            if (hostEnvironment.IsDevelopment())
            {
                application.UseDeveloperExceptionPage();                
            }

            application.UseSwagger();
            application.UseSwaggerUI(c => {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApiAutores v1");
                c.SwaggerEndpoint("/swagger/v2/swagger.json", "WebApiAutores v2");
            });

            application.UseHttpsRedirection();
            application.UseRouting();

            application.UseCors();

            application.UseLimitarPeticiones();
            
            application.UseAuthorization();

            application.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
