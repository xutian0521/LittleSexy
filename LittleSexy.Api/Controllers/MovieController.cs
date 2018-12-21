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
    public class MovieController : ControllerBase
    {
        protected MovieService _service;
        public MovieController(MovieService movieService)
        {
            _service = movieService;
        }
        [HttpGet("List")]
        public async Task<ApiResult> List()
        {
            var result = await _service.GetMoviesListAsync();
            Response.StatusCode= 200;
            return result;
        }

    }
}