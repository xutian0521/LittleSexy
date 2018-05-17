using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LittleSexy.Common;
using Microsoft.Extensions.DependencyInjection;
using LittleSexy.Service;
namespace LittleSexy.Api.Controllers
{
    public class PageController : Controller
    {
        protected PageService _pageService;
        public PageController(IServiceProvider service)
        {
            _pageService = service.GetService<PageService>();
        }
        public async Task<ApiResult> GetPageContent(int pageId)
        {
            var list = await _pageService.GetPageContentPageIdAsync(pageId);
            ApiResult result = new ApiResult();
            result.Code = 200;
            result.Message = "成功";
            result.Content = list;
            return result;
        }
        public async Task<ApiResult> GetPageImages(int pageId)
        {
            var list = await _pageService.GetPageImagesByPageIdAsync(pageId);
            ApiResult result = new ApiResult();
            result.Code = 200;
            result.Message = "成功";
            result.Content = list;
            return result;
        }

    }
}