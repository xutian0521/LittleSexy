using LittleSexy.Common;
using LittleSexy.Model.DBModel;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using LittleSexy.DAL;
using System.Threading.Tasks;
using LittleSexy.Model.ViewModel;

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
        public async Task<t_Page> GetPageContentPageIdAsync(int pageId)
        {
            var list = await _pageDAL.GetDataPageContentByPageIdAsync( pageId);
            return list;
        }
        public async Task<IEnumerable<v_PageImages>> GetPageImagesByPageIdAsync(int pageId)
        {
            var list = await _pageDAL.GetDataPageImagesByPageIdAsync(pageId);
            return list;
        }
    }

}
