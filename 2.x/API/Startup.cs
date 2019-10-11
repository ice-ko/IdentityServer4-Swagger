using System;
using System.Collections.Generic;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

namespace TPL.API
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            //用户校验
            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
              .AddIdentityServerAuthentication(options =>
              {
                  options.Authority = "http://localhost:5000"; // IdentityServer服务器地址
                  options.ApiName = "demo_api"; // 用于针对进行身份验证的API资源的名称
                  options.RequireHttpsMetadata = false; // 指定是否为HTTPS
              });
            //添加Swagger.
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Info { Title = "Protected API", Version = "v1" });
                //向生成的Swagger添加一个或多个“securityDefinitions”，用于API的登录校验
                options.AddSecurityDefinition("oauth2", new OAuth2Scheme
                {
                    Flow = "implicit", // 只需通过浏览器获取令牌（适用于swagger）
                    AuthorizationUrl = "http://localhost:5000/connect/authorize",//获取登录授权接口
                    Scopes = new Dictionary<string, string> {
                        { "demo_api", "请选择授权API" }//指定客户端请求的api作用域。 如果为空，则客户端无法访问
                    }
                });

                options.OperationFilter<AuthorizeCheckOperationFilter>(); // 添加IdentityServer4认证过滤
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseAuthentication();

            // Swagger JSON Doc
            app.UseSwagger();

            // Swagger UI
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                options.OAuthClientId("demo_api_swagger");//客服端名称
                options.OAuthAppName("Demo API - Swagger-演示"); // 描述
            });
            app.UseMvc();
        }
    }
}
