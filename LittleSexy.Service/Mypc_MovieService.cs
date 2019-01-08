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
    public class Mypc_MovieService
    {
        //todo:业务层改用多个实例
        public IConfiguration Configuration { get; }
        protected PageDAL _pageDAL;
        public Mypc_MovieService(IServiceProvider service,IConfiguration configuration)
        {
            _pageDAL = service.GetService<PageDAL>();
            Configuration = configuration;
        }
        public async Task<ApiResult> GetMoviesListAsync()
        {
            ApiResult result=new ApiResult();
            string movieRootPath = Configuration.GetSection("movieRootPath").Value;
            string movieHttpUrl = Configuration.GetSection("movieHttpUrl").Value;
            
            List<FileInfo> fileList = new List<FileInfo>();
            if(Directory.Exists(movieRootPath))
            {
                this.GetAllDir(movieRootPath, ref fileList);

            }
            else
            {
                return new ApiResult(-1, $@"没找到磁盘上{movieRootPath}的目录");
            }
            List<v_Movie> movieLists=new List<v_Movie>();
            foreach (var item in fileList)
            {
                v_Movie movie=new v_Movie();
                movie.Title =Path.GetFileName(item.FullName);
                movie.FanHao=item.FullName.Split('_')[1];
                //movie.LinkUrl = "movieDetail.html?src=" + System.Net.WebUtility.UrlEncode(movieHttpUrl+item.FullName);
                movie.LinkUrl = "movieDetail.html?src=" + System.Net.WebUtility.UrlEncode("https://vjs.zencdn.net/v/oceans.webm");
                movie.Cover = "/images/hamburger.50e4091.png";
                movie.Date = item.CreationTime.ToString("yyyy-MM-dd hh:mm");
                movieLists.Add(movie);
            }
            result.Content= movieLists;
            return result;
        }
        public  void GetAllDir(string dir1,ref List<FileInfo> fileList)
        {
            DirectoryInfo dir = new DirectoryInfo(dir1);
            var files = dir.GetFiles();
            var dirs =dir.GetDirectories();
            for (int i = 0; i < files.Length; i++)
            {
                var ext= Path.GetExtension( files[i].FullName);
                if(ext ==".webm" || ext == ".mp4")
                {
                    fileList.Add(files[i]);
                }
            }
            for (int i = 0; i < dirs.Length; i++)
            {
                if(dirs.Length > 0)
                    GetAllDir(dirs[i].FullName, ref fileList);
            }
            
            for (int i = 0; i < fileList.Count; i++)
            {
                Console.WriteLine(fileList[i]);
            }
        }
    }

}
