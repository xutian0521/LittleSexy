using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.IO;
using LittleSexy.Api.Models;

namespace LittleSexy.Api.Services
{
    public class HomeService
    {
        public IConfiguration _configuration { get; }
        public HomeService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<List<v_PageImages>> GetBnners()
        {
            List<v_PageImages> vmls = new List<v_PageImages>();
            string currentPath = Directory.GetCurrentDirectory();
            DirectoryInfo dir = new DirectoryInfo(currentPath + "/wwwroot/images/homeBanner/");
            var files = dir.GetFiles();
            string host = _configuration.GetValue<string>("ApiHost") + "/images/homeBanner/";
            foreach (var item in files)
            {
                vmls.Add(new v_PageImages { ImagePath = host + item.Name });
            }
            return vmls;
        }
    }
}
