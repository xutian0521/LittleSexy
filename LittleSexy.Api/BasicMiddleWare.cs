using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace LittleSexy.Api
{
    /// <summary>
    /// 服务执行中间层
    /// </summary>
    internal class BasicMiddleWare
    {
        private readonly RequestDelegate _next;

        public BasicMiddleWare(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context, IServiceProvider serviceProvider)
        {

            context.Response.Headers.Add("Access-Control-Allow-Headers", "x-requested-with,content-type,Authorization");
            context.Response.Headers.Add("Access-Control-Allow-Methods", "POST,GET,OPTIONS,DELETE");
            context.Response.Headers.Add("Access-Control-Request-Methods", "POST,GET,OPTIONS,DELETE");
            context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            context.Response.Headers.Add("Access-Control-Allow-Credentials", "true");
            if (context.Request.Method.ToLower() == "options")
            {
                context.Response.StatusCode = 202;
                return;
            }
            await _next.Invoke(context);
        }
    }
}