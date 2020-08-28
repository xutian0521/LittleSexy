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
    public class MovieController : ControllerBase
    {
        protected MovieService _service;
        public MovieController(MovieService movieService)
        {
            _service = movieService;
        }
        [HttpGet("List")]
        public async Task<List<v_Movie>> List(int pageIndex = 1, int pageSize =20, string sort = "CreateTime")
        {
            var result = await _service.GetList(pageIndex, pageSize, sort);
            return result;
        }
        [HttpGet("Detail")]
        public async Task<v_Movie> Detail(int id)
        {
            var result = await _service.DetailAsync(id);
            return result;
        }

        [HttpGet("Actresses")]
        public async Task<List<v_Actress>> Actresses()
        {
            var result = await _service.Actresses();
            return result;
        }

    }
}