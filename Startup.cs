using System;
using System.Text;
using AlgorithmEasy.Server.ProjectCenter.Services;
using AlgorithmEasy.Shared.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace AlgorithmEasy.Server.ProjectCenter
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

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Server.ProjectCenter", Version = "v1" });
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = Environment.GetEnvironmentVariable("ALGORITHMEASY_SECURITY_TOKENS_ISSUER"),
                        ValidateAudience = true,
                        ValidAudience = Environment.GetEnvironmentVariable("ALGORITHMEASY_SECURITY_TOKENS_AUDIENCE"),
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(
                                Environment.GetEnvironmentVariable("ALGORITHMEASY_SECURITY_TOKENS_KEY")!))
                    };
                });

            var connection = Environment.GetEnvironmentVariable("ALGORITHMEASY_DB_CONNECTION_STRING");
            var version = ServerVersion.AutoDetect(connection);
            services.AddDbContext<AlgorithmEasyDbContext>(options => options.UseMySql(connection!, version));

            services.AddScoped<ProjectManagerService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Server.ProjectCenter v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
