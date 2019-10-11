using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };
        }
        /// <summary>
        /// API信息
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ApiResource> GetApis()
        {
            return new[]
            {
                new ApiResource("api", "Demo API with Swagger")
            };
        }
        /// <summary>
        /// 客服端信息
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Client> GetClients()
        {
            return new[]
            {
                new Client
                {
                    ClientId = "api_swagger",//客服端名称
                    ClientName = "Swagger UI for demo_api",//描述
                    AllowedGrantTypes = GrantTypes.Implicit,//指定允许的授权类型（AuthorizationCode，Implicit，Hybrid，ResourceOwner，ClientCredentials的合法组合）。
                    AllowAccessTokensViaBrowser = true,//是否通过浏览器为此客户端传输访问令牌
                    RedirectUris =
                    {
                        "http://localhost:5001/swagger/oauth2-redirect.html"
                    },
                    AllowedScopes = { "api" }//指定客户端请求的api作用域。 如果为空，则客户端无法访问
                },
                 //定义mvc客户端
                new Client
                {
                    //客户端ID名称
                    ClientId = "mvc",
                    ClientName = "MVC Client",
                    //访问类型
                    AllowedGrantTypes = GrantTypes.Hybrid,
                    //关闭确认是否返回身份信息界面
                    RequireConsent=false,
                   // 登录成功后重定向地址
                    RedirectUris = { "http://localhost:5002/signin-oidc" },
                   //注销成功后的重定向地址
                    PostLogoutRedirectUris = { "http://localhost:5002/signout-callback-oidc" },
                     //用于认证的密码加密方式
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                     //客户端有权访问的范围
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "api",//要访问的api名称
                    },
                }
            };
        }
    }
}
