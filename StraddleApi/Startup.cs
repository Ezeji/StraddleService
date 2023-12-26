using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using StraddleCore.Models;
using StraddleData;
using System.Configuration;
using StraddleRepository;
using StraddleCore.Services;
using StraddlePaymentApi.Middleware;
using StraddleCore.Configurations.Azure;
using System.Text;

namespace StraddleApi
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
            //we keep using NewtonSoft so that serialization of reference loop can be ignored, especially because of EFCore
            services.AddControllers()
                .AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore)
                .AddNewtonsoftJson(x => x.SerializerSettings.Converters.Add(new StringEnumConverter()));

            services.AddAutoMapper(typeof(Startup), typeof(JwtConfig));

            services.AddDbContext<StraddleDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("StraddleDb"), b => b.MigrationsAssembly("StraddleData"));
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                //For OpenId Connect tokens
                options.Authority = Configuration["JwtConfig:Issuer2"];
                options.Audience = Configuration["JwtConfig:Issuer2"];
                options.SaveToken = true;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    //ValidIssuer = Configuration["JwtConfig:Issuer"],
                    ValidIssuers = new[] { Configuration["JwtConfig:Issuer"], Configuration["JwtConfig:Issuer2"] },
                    //ValidAudience = Configuration["JwtConfig:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtConfig:Key"]))
                };

            });

            services.AddCors(option =>
            {
                option.AddDefaultPolicy(
                    builder =>
                    {
                        builder
                        .AllowAnyOrigin()
                        .SetIsOriginAllowedToAllowWildcardSubdomains()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                    });
            });

            services.AddHttpClient();
            services.AddHttpContextAccessor();

            //add external configurations
            services.AddConfigurations();

            //Add custom Core Repo and Service
            services.AddCoreRepository();
            services.AddServices();

            //Add our configs
            services.AddConfigSettings(Configuration);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Straddle Service", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
                app.UseMiddleware<CustomErrorHandlerMiddleware>(); //custom error handler
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors();

            if (env.IsDevelopment() || env.IsStaging())
            {
                app.UseSwagger();
                app.UseSwaggerUI(opt =>
                {
                    opt.SwaggerEndpoint("/swagger/v1/swagger.json", "Straddle Service");
                });
            }

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

    }
}
