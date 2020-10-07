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
        [HttpPut]
        public IActionResult UpdateAllMovies()
        {
            var result =  _service.UpdateAllMovies();
            if (result)
            {
                return Accepted();
            }
            else
            {
                return NotFound();
            }

        }
        [HttpGet("List")]
        public  List<v_Movie> MovieList
            (string sort = "CreateTime", string actressName =null, int? isLiked = null, int pageIndex = 1, int pageSize =20 )
        {
            var result = _service.GetList( sort, actressName, isLiked, pageIndex, pageSize);
            return result;
        }

        [HttpGet("Detail")]
        public v_Movie MovieDetail(int id)
        {
            var result = _service.DetailAsync(id);
            return result;
        }

        [HttpGet("Actresses")]
        public List<v_Actress> Actresses(string sort = "CreateTime", int? isLiked = null)
        {
            var result = _service.Actresses(sort, isLiked);
            return result;
        }

        [HttpGet("ActressDetail")]
        public v_Actress ActressDetail(string actressName)
        {
            var result = _service.ActressDetails(actressName);
            return result;
        }
        
        [HttpPatch("LikingMovie")]
        public IActionResult LikingMovie(int id, int isLiked)
        {
            var result = _service.LikingMovie(id, isLiked);
            if (result)
            {
                return Accepted();
            }
            else
            {
                return NotFound();
            }
        }
        [HttpPatch("LikingActress")]
        public IActionResult LikingActress(string actressName, int isLiked)
        {
            var result =  _service.LikingActress(actressName, isLiked);
            if (result)
            {
                return Accepted();
            }
            else
            {
                return NotFound();
            }
        }

    }
}