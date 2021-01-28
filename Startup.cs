using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SSEDotnet;

namespace SSEServer {
    public class Startup {
        public Startup(IConfiguration configuration){
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services){
            services.AddSingleton<SseService>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(
                    jwtBearerOptions => {
                        jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters{
                            ValidateActor = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = "Issuer",
                            ValidAudience = "Audience",
                            IssuerSigningKey = new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes("Hlkjds0-324mf34pojf-34r34fwlknef0943"))
                        };
                    });
            IdentityModelEventSource.ShowPII = true;

            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new OpenApiInfo{Title = "SSEServer", Version = "v1"});
                c.AddSecurityDefinition("Bearer",
                    new OpenApiSecurityScheme{
                        In = ParameterLocation.Header,
                        Description =
                            "Please enter into field the word 'Bearer' following by space and JWT",
                        Name = "authorization",
                        Type = SecuritySchemeType.ApiKey
                    });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement{
                    {
                        new OpenApiSecurityScheme{
                            Reference =
                                new OpenApiReference{
                                    Type = ReferenceType.SecurityScheme, Id = "Bearer"
                                },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });
            });
            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
            SseService sseService){
            if (env.IsDevelopment()){
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SSEServer v1"));
            }

            sseService.CreateMethodsMap(Assembly.GetExecutingAssembly());

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();
            app.UseAuthentication();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}