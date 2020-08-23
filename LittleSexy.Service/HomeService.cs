using LittleSexy.Common;
using LittleSexy.Model.DBModel;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using LittleSexy.DAL;
using System.Threading.Tasks;
using LittleSexy.Model.ViewModel;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.IO;

namespace LittleSexy.Service
{
    [Inject]
    public class HomeService
    {
        public IConfiguration _configuration { get; }
        public HomeService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<IEnumerable<v_PageImages>> GetBnners()
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
