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
using System.IO;

namespace LittleSexy.Service
{
    [Inject]
    public class MovieService
    {
        public IConfiguration Configuration { get; }
        protected PageDAL _pageDAL;
        public MovieService(IServiceProvider service,IConfiguration configuration)
        {
            _pageDAL = service.GetService<PageDAL>();
            Configuration = configuration;
        }
        public async Task<t_Page> GetMoviesListAsync()
        {
            string movieRootPath=@"H:\ftp";
            if(Directory.Exists(movieRootPath))
            {
                
            }
            int pageId=1;
            var list = await _pageDAL.GetDataPageContentByPageIdAsync( pageId);
            return list;
        }
    }

}
