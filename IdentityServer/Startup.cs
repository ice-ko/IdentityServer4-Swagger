using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Quickstart.UI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            //配置身份服务器与内存中的存储，密钥，客户端和资源
            services.AddIdentityServer()
                   .AddDeveloperSigningCredential()
                   .AddInMemoryApiResources(Config.GetApis())//添加api资源
                   .AddInMemoryClients(Config.GetClients())//添加客户端
                   .AddInMemoryIdentityResources(Config.GetIdentityResources())//添加对OpenID Connect的支持
                   .AddTestUsers(TestUsers.Users); //添加测试用户
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //IdentityServe
            app.UseIdentityServer();
            //添加静态资源访问
            app.UseStaticFiles();
            //
            app.UseMvcWithDefaultRoute();
        }
    }
}
