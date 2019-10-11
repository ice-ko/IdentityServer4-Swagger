using System;
using System.Collections.Generic;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;

namespace TPL.API
{
    public class Startup
    {

        public IWebHostEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            //用户校验
            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
              .AddIdentityServerAuthentication(options =>
              {
                  options.Authority = "http://localhost:5000"; // IdentityServer服务器地址
                  options.ApiName = "api"; // 用于针对进行身份验证的API资源的名称
                  options.RequireHttpsMetadata = false; // 指定是否为HTTPS
              });
            //添加Swagger.
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Protected API", Version = "v1" });
                //向生成的Swagger添加一个或多个“securityDefinitions”，用于API的登录校验
                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        Implicit = new OpenApiOAuthFlow
                        {
                            //授权地址
                            AuthorizationUrl = new Uri("http://localhost:5000/connect/authorize"),
                            Scopes = new Dictionary<string, string>
                            {
                                { "api", "请选择授权API" },
                            }
                        }
                    }

                });

                options.OperationFilter<AuthorizeCheckOperationFilter>(); // 添加IdentityServer4认证过滤
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            // Swagger JSON Doc
            app.UseSwagger();

            // Swagger UI
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                options.OAuthClientId("api_swagger");//客服端名称
                options.OAuthAppName("Demo API - Swagger-演示"); // 描述
            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
