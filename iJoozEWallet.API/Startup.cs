using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Security.Cryptography.X509Certificates;
using AutoMapper;
using IdentityServer4.AccessTokenValidation;
using iJoozEWallet.API.Domain.Repositories;
using iJoozEWallet.API.Domain.Services;
using iJoozEWallet.API.Persistence.Contexts;
using iJoozEWallet.API.Persistence.Repositories;
using iJoozEWallet.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MySql.Data.MySqlClient;

namespace iJoozEWallet.API
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
            CommonConfig(services);

            services.AddDbContext<AppDbContext>(options =>
                options.UseMySql(GetDbConnection().ConnectionString));
        }

        DbConnection GetDbConnection()
        {
            var connectionString = new MySqlConnectionStringBuilder(
                Environment.GetEnvironmentVariable("DbConnectionString"))
            {
                // Connecting to a local proxy that does not support ssl.
                SslMode = MySqlSslMode.None,
            };
            DbConnection connection = new MySqlConnection(connectionString.ConnectionString);

            return connection;
        }


        public void ConfigureDevelopmentServices(IServiceCollection services)
        {
            CommonConfig(services);
            services.AddDbContext<AppDbContext>(options => { options.UseInMemoryDatabase("ijooz-db-in-memory"); });
        }


        private void CommonConfig(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddScoped<IEWalletRepository, EWalletRepository>();
            services.AddScoped<IEWalletService, EWalletService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<TokenService, TokenService>();

            services.AddAutoMapper();

            SwaggerConfig(services);
            VerifyAuthenticationWithAuthServer(services);
            AddCorsPolicy(services);
        }

        private static void AddCorsPolicy(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowMyOrigin",
                    builder => builder
                        .WithOrigins("https://fvmembership-ui.web.app", "https://localhost:8100")
                        .AllowAnyHeader()
                );
            });
        }

        private static void VerifyAuthenticationLocally(IServiceCollection services)
        {
            byte[] signingKey = Convert.FromBase64String(Environment.GetEnvironmentVariable("SigningKey"));
            var issuerSigningKey = new X509SecurityKey(new X509Certificate2(new X509Certificate(signingKey)));

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                ValidateIssuer = false,
                IssuerSigningKey = issuerSigningKey
            };
            services.AddAuthentication()
                .AddJwtBearer(options => { options.TokenValidationParameters = tokenValidationParameters; });
            services.AddAuthorization(options =>
            {
                options.DefaultPolicy =
                    new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                        .RequireAuthenticatedUser()
                        .Build();
            });
        }

        private void VerifyAuthenticationWithAuthServer(IServiceCollection services)
        {
            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = Configuration["AuthHost"];
                    options.ApiName = "FvMembership";
                    options.RequireHttpsMetadata = false;
                    options.EnableCaching = true;
                    options.CacheDuration = TimeSpan.FromMinutes(10);
                })
                ;
        }

        private static void SwaggerConfig(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.EnableAnnotations();
                c.SwaggerDoc("v1", new OpenApiInfo()
                {
                    Version = "v1",
                    Title = "E-Wallet API",
                    Description = "E-Wallet API ",
                    Contact = new OpenApiContact {Name = "Zou Xuan", Email = "xuanozu89@gmail.com"}
                });
                var openApiSecurityScheme = new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter into field the word 'Bearer' following by space and JWT",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                };
                c.AddSecurityDefinition("Bearer", openApiSecurityScheme);

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
                        new List<string>()
                    }
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseCors("AllowMyOrigin");
            loggerFactory.AddLog4Net();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(options => { options.SwaggerEndpoint("/swagger/v1/swagger.json", "EWallet API V1"); });
        }
    }
}