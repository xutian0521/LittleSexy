using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Cors;
using System.Reflection;
using LittleSexy.Common;
using Microsoft.AspNetCore.HttpOverrides;

namespace LittleSexy.Api
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
            services.AddMvc();
            services.AddCors();
        
            List<Assembly> listAsb = new List<Assembly>();
            listAsb.Add(Assembly.Load(new AssemblyName("LittleSexy.Service")));
            listAsb.Add(Assembly.Load(new AssemblyName("LittleSexy.DAL")));
            foreach (Assembly asb in listAsb)
            {
                foreach (var item in asb.GetTypes().Where(t => !t.IsAbstract && 
                    t.IsDefined(typeof(InjectAttribute))))
                {
                    services.AddSingleton(item);
                }
            }
            //test,Desktop computer,aliyun
            string environment= Configuration.GetSection("environment").Value;
            if(environment =="test")
            {
                services.AddSingleton(typeof(Service.Interface.IMovieService),typeof(Service.MovieService));
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {


                app.UseDeveloperExceptionPage();
            }
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            app.UseStaticFiles();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
            
            app.UseCors(builder =>
                builder.WithOrigins("http://localhost:5008").AllowAnyHeader()
                );
        }
    }
}
