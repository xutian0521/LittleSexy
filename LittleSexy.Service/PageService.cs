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

namespace LittleSexy.Service
{
    [Inject]
    public class PageService
    {
        protected PageDAL _pageDAL;
        public PageService(IServiceProvider service)
        {
            _pageDAL = service.GetService<PageDAL>();
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
            string ossHost =JsonConfigurationHelper.GetAppSettings("osshost");
            foreach (var item in list)
            {
                item.ImagePath = ossHost + item.ImagePath;
            }
            return list;
        }
    }

}
