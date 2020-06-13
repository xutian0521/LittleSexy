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
    public class PageService
    {
        IConfiguration _configuration;
        protected PageDAL _pageDAL;
        public PageService(IServiceProvider service, IConfiguration configuration)
        {
            _pageDAL = service.GetService<PageDAL>();
            _configuration = configuration;
        }
        public async Task<t_Page> GetPageContentFormTabAsync(string  tab)
        {
            int pageId=1;
            switch (tab)
            {
                default:
                case "Index":
                    pageId = 1;
                break;
                
            }
            var list = await _pageDAL.GetDataPageContentByPageIdAsync( pageId);
            return list;
        }
        public async Task<IEnumerable<v_PageImages>> GetPageImagesByPageIdAsync(int pageId)
        {
            var list = await _pageDAL.GetDataPageImagesByPageIdAsync(pageId);
            string ossHost = JsonConfigurationHelper.GetAppSettings("osshost");
            foreach (var item in list)
            {
                item.ImagePath = ossHost + item.ImagePath;
            }
            return list;
        }

        public async Task<IEnumerable<v_PageImages>> GetHomeViewData(int pageId)
        {
            List<v_PageImages> vmls = new List<v_PageImages>();
            string currentPath = Directory.GetCurrentDirectory();
            DirectoryInfo dir = new DirectoryInfo(currentPath + "/wwwroot/images/homeBanner/");
            var files = dir.GetFiles();
            string host = "http://localhost:5008/images/homeBanner/";
            foreach (var item in files)
            {
                vmls.Add(new v_PageImages { ImagePath = host + item.Name });
            }

            return vmls;
        }
    }

}
