using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LittleSexy.Api.Models;
using LittleSexy.Api.Services;
using LittleSexy.Api.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace LittleSexy.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        protected HomeService _homeService;
        public HomeController(HomeService homeService)
        {
            _homeService = homeService;
        }
        [HttpGet("Banner")]
        public async Task<List<v_PageImages>> Banner(int pageId)
        {
            var list = await _homeService.GetBnners();
            return list;
        }
    }
}
