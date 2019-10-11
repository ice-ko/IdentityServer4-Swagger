using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Newtonsoft.Json.Linq;
using Web.Models;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Privacy()
        {
            //获取用户信息
            var claimIdentity = (ClaimsIdentity)HttpContext.User.Identity;
            var claimsPrincipal = claimIdentity.Claims as List<Claim>;
            //获取用户token
            var token = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            //实例化HttpClient
            var client = new HttpClient();
            //设置token
            client.SetBearerToken(token);
            //请求identity接口
            var response = await client.GetAsync("http://localhost:5000/identity");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
            }
            var content = await response.Content.ReadAsStringAsync();
            return View(JArray.Parse(content));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
       
        /// <summary>
        /// 注销
        /// </summary>
        /// <returns></returns>
        public async Task Logout()
        {
            await HttpContext.SignOutAsync("Cookies");
            await HttpContext.SignOutAsync("oidc");
        }

        //public async Task UserIinfo()
        //{
        //    var client = new HttpClient();
        //    var disco = await client.GetDiscoveryDocumentAsync("http://localhost:5000");
        //    if (disco.IsError)
        //    {
        //        Console.WriteLine(disco.Error);
        //        return;
        //    }
        //    var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
        //    {
        //        Address = disco.TokenEndpoint,

        //        ClientId = "client",
        //        ClientSecret = "secret",
        //        Scope = "api1"
        //    });


        //    if (tokenResponse.IsError)
        //    {
        //        Console.WriteLine(tokenResponse.Error);
        //        return;
        //    }

        //    Console.WriteLine(tokenResponse.Json);
        //    // call api
        //    client.SetBearerToken(tokenResponse.AccessToken);

        //    var response = await client.GetAsync("http://localhost:5001/identity");
        //    if (!response.IsSuccessStatusCode)
        //    {
        //        Console.WriteLine(response.StatusCode);
        //    }
        //    else
        //    {
        //        var content = await response.Content.ReadAsStringAsync();
        //        Console.WriteLine(JArray.Parse(content));
        //    }
        //}
    }
}
