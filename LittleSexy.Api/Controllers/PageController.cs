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
    [Route("api/[controller]")]
    [ApiController]
    public class PageController : ControllerBase
    {
        protected PageService _pageService;
        public PageController(PageService pageService)
        {
            _pageService = pageService;
        }
        [HttpGet("{tab}")]
        public async Task<ApiResult> Get(string tab)
        {
            var list = await _pageService.GetPageContentPageIdAsync(tab);
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