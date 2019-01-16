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
using LittleSexy.Service.Interface;

namespace LittleSexy.Service
{
    [Inject]
    public class Mypc_MovieService: IMovieService
    {
        //todo:业务层改用多个实例
        public IConfiguration Configuration { get; }
        protected PageDAL _pageDAL;
        protected MovieDAL _movieDAL;
        public Mypc_MovieService(IServiceProvider service,IConfiguration configuration)
        {
            _pageDAL = service.GetService<PageDAL>();
            _movieDAL=service.GetService<MovieDAL>();
            Configuration = configuration;
        }
        public async Task<ApiResult> UpdateMoviesListAsync()
        {
            ApiResult result=new ApiResult();
            string movieRootPath = Configuration.GetSection("movieRootPath").Value;
            List<List<FileInfo>> fileList = new List<List<FileInfo>>();
            if(Directory.Exists(movieRootPath))
            {
                this.GetAllDir(movieRootPath, ref fileList);
            }
            else
            {
                return new ApiResult(-1, $@"没找到磁盘上{movieRootPath}的目录");
            }
            List<t_Movie> movieLists=new List<t_Movie>();
            foreach (var itemOnes in fileList)
            {
                t_Movie movie=new t_Movie();
                foreach (var item in itemOnes)
                {
                    var ext= Path.GetExtension( item.FullName);
                    if(ext ==".mp4" || ext == ".webm")
                    {
                        movie.Title =Path.GetFileName(item.FullName);
                        movie.FanHao=item.FullName.Split('_')[1];
                        movie.Source = Path.GetFileName(item.FullName);

                        movie.CreationTime = item.CreationTime;
                    }
                    if(ext == ".jpg" || ext == ".png")
                    {
                        movie.Cover = "/" + movie.FanHao + ".jpg";
                    }
                }
                movieLists.Add(movie);
            }
            int row= await _movieDAL.InsertMovieListAsync(movieLists);
            result.Content= movieLists;
            return result;
        }
        public async Task<ApiResult> GetMoviesListAsync(int pageIndex, int pageSize)
        {
            string movieRootPath = Configuration.GetSection("movieRootPath").Value;
            ApiResult result=new ApiResult();
            List<v_Movie> movieLists=new List<v_Movie>();
            IEnumerable<t_Movie> lsMovie= await _movieDAL.GetMovieListAsync(pageIndex, pageSize);
            foreach (var item in lsMovie)
            {
                v_Movie model=new v_Movie();
                model.Title= item.Title;
                model.FanHao = item.FanHao;
                model.Cover = item.Cover;
                model.LinkUrl = "movieDetail?Id=" +item.Id;
                model.Source = movieRootPath+ item.Source;
                model.Date =item.CreationTime.ToString("yyyy-MM-dd hh:mm");
                movieLists.Add(model);
            }
            result.Content= movieLists;
            return result;
        }
        public  void GetAllDir(string dir1,ref List<List<FileInfo>> fileList)
        {
            DirectoryInfo dir = new DirectoryInfo(dir1);
            var files = dir.GetFiles();
            var dirs =dir.GetDirectories();
            
            for (int i = 0; i < files.Length; i++)
            {
                List<FileInfo> ones=new List<FileInfo>();
                var ext= Path.GetExtension( files[i].FullName);
                if(ext == ".jpg" || ext == ".png")
                {
                    ones.Add(files[i]);
                }
                if(ext ==".webm" || ext == ".mp4")
                {
                    ones.Add(files[i]);
                    fileList.Add(ones);
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
