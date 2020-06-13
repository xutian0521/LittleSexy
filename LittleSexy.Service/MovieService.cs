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
using Microsoft.Extensions.Caching.Memory;
using System.Linq;

namespace LittleSexy.Service
{
    [Inject]
    public class MovieService
    {
        public static IMemoryCache _memoryCache = new MemoryCache(new MemoryCacheOptions());
        //todo:业务层改用多个实例
        public IConfiguration _configuration { get; }
        protected PageDAL _pageDAL;
        protected MovieDAL _movieDAL;
        public MovieService(IServiceProvider service,IConfiguration configuration)
        {
            _pageDAL = service.GetService<PageDAL>();
            _movieDAL=service.GetService<MovieDAL>();
            _configuration = configuration;
        }

        /// <summary>
        /// 获取电影列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页容量</param>
        /// <returns></returns>
        public async Task<ApiResult> GetMoviesDetailAsync(long id)
        {
            string movieRootPath = _configuration.GetSection("movieRootPath").Value;
            string imageServerHost= _configuration.GetSection("imageServerHost").Value;
            ApiResult result=new ApiResult();
            List<v_Movie> movieLists=new List<v_Movie>();
            t_Movie item= await _movieDAL.GetMovieOneAsync(id);
                v_Movie model=new v_Movie();
                model.Title= item.Title;
                model.FanHao = item.FanHao;
                model.Cover = imageServerHost + item.Cover.Replace( "\\", "/");
                model.LinkUrl = "movieDetail?Id=" +item.Id;
                model.Source = imageServerHost + item.Source;
                model.Date =item.CreationTime.ToString("yyyy-MM-dd hh:mm");
            result.Content= model;
            return result;
        }
        
        /// <summary>
        /// 获取电影列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页容量</param>
        /// <returns></returns>
        public async Task<ApiResult> GetMoviesListAsync(int pageIndex, int pageSize)
        {

            string movieRootPath = _configuration.GetSection("movieRootPath").Value;
            string imageServerHost= _configuration.GetSection("imageServerHost").Value;
            ApiResult result=new ApiResult();
            List<v_Movie> movieList=new List<v_Movie>();
            IEnumerable<t_Movie> lsMovie= await _movieDAL.GetMovieListAsync(pageIndex, pageSize);
            foreach (var item in lsMovie)
            {
                v_Movie model=new v_Movie();
                model.Id = item.Id;
                model.Title= item.Title;
                model.FanHao = item.FanHao;
                model.Cover = imageServerHost + item.Cover.Replace( "\\", "/");
                model.LinkUrl = "/#/Movie/Detail?Id=" +item.Id;
                model.Source = movieRootPath+ item.Source;
                model.Date =item.CreationTime.ToString("yyyy-MM-dd hh:mm");
                movieList.Add(model);
            }
            result.Content= movieList;

            return result;
        }

        /// <summary>
        /// 更新文件目录列表
        /// </summary>
        /// <returns></returns>
        public async Task<ApiResult> UpdateMoviesListAsync()
        {
            ApiResult result=new ApiResult();
            string movieRootPath = _configuration.GetSection("movieRootPath").Value;
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
                        movie.Title =Path.GetFileNameWithoutExtension(item.Name);
                        movie.FanHao=Path.GetFileNameWithoutExtension(item.Name);
                        movie.Source = item.FullName.Replace(movieRootPath, "");

                        movie.CreationTime = item.CreationTime;
                    }
                }

                foreach (var item in itemOnes)
                {
                    var ext= Path.GetExtension( item.FullName);
                    if(ext == ".jpg" || ext == ".png")
                    {
                        string picRelativePath = item.FullName.Replace(movieRootPath, "");

                        if (item.Name.Contains(movie.FanHao))
                        {
                            movie.Cover = picRelativePath;
                            movie.Title = Path.GetFileNameWithoutExtension(item.Name);
                        }
                        else
                        {
                            movie.Details += picRelativePath + ";";
                        }
                    }
                }
                movieLists.Add(movie);
            }
            int row= await _movieDAL.UpdateInsertMovieListAsync(movieLists);
            result.Content= movieLists;
            return result;
        }
        /// <summary>
        /// 递归获取电影文件
        /// </summary>
        /// <param name="dir1"></param>
        /// <param name="fileList"></param>
        private  void GetAllDir(string dir1,ref List<List<FileInfo>> fileList)
        {
            DirectoryInfo dir = new DirectoryInfo(dir1);
            var files = dir.GetFiles();
            var dirs =dir.GetDirectories();
            
            List<FileInfo> ones=new List<FileInfo>();
            for (int i = 0; i < files.Length; i++)
            {
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
            
            for (int i = 0; i < ones.Count; i++)
            {
                Console.WriteLine(ones[i].FullName);
            }
        }

        public async Task<ApiResult> GetList(int pageIndex, int pageSize)
        {
            var _cache = _memoryCache.Get<List<v_Movie>>("movieTempList");
            if (_cache != null && _cache.Count > 0)
            {
                return new ApiResult() { Content = _cache };
            }
            ApiResult result = new ApiResult();

            string movieRootPath = Directory.GetCurrentDirectory() + @"\wwwroot\ftp\";
            List<List<FileInfo>> fileList = new List<List<FileInfo>>();
            if (Directory.Exists(movieRootPath))
            {
                this.GetAllDir(movieRootPath, ref fileList);
            }
            else
            {
                return new ApiResult(-1, $@"没找到磁盘上{movieRootPath}的目录");
            }
            List<t_Movie> movieLists = new List<t_Movie>();
            foreach (var itemOnes in fileList)
            {
                t_Movie movie = new t_Movie();
                foreach (var item in itemOnes)
                {
                    var ext = Path.GetExtension(item.FullName);
                    if (ext == ".mp4" || ext == ".webm")
                    {
                        movie.Title = item.Name;
                        movie.FanHao = Path.GetFileNameWithoutExtension(item.Name);
                        movie.Source = item.FullName.Replace(movieRootPath, "");

                        movie.CreationTime = item.CreationTime;
                    }
                }

                foreach (var item in itemOnes)
                {
                    var ext = Path.GetExtension(item.FullName);
                    if (ext == ".jpg" || ext == ".png")
                    {
                        string picRelativePath = item.FullName.Replace(movieRootPath, "");

                        if (item.Name.Contains(movie.FanHao))
                        {
                            movie.Cover = picRelativePath;
                            movie.Title = Path.GetFileNameWithoutExtension(item.Name);
                        }
                        else
                        {
                            movie.Details += picRelativePath + ";";
                        }
                    }
                }
                movieLists.Add(movie);
            }
            string AppHost = _configuration.GetValue<string>("AppHosts");
            List<v_Movie> lsMovie = new List<v_Movie>();
            foreach (var item in movieLists)
            {
                v_Movie model = new v_Movie();
                model.Id = lsMovie.Count +1;
                model.Title = item.Title;
                model.FanHao = item.FanHao;
                model.Cover = AppHost + @"/ftp/" + item.Cover?.Replace("\\", "/");
                model.LinkUrl = "{path:'movie/detail', query: { id: " + model.Id + " }}";
                model.Source = AppHost+ @"/ftp/" + item.Source;
                model.Date = item.CreationTime.ToString("yyyy-MM-dd hh:mm");
                lsMovie.Add(model);
            }
            result.Content = lsMovie;
            _memoryCache.Set("movieTempList", lsMovie, new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(30)));
            return result;
        }

        public async Task<ApiResult> DetailAsync(long id)
        {
            var _cache = _memoryCache.Get<List<v_Movie>>("movieTempList");

            ApiResult result = new ApiResult();
            List<v_Movie> movieLists = new List<v_Movie>();

            if (_cache != null && _cache.Count > 0)
            {
                result.Content = _cache.Where(x => x.Id == id).FirstOrDefault(); ;
            }
            return result;
        }
    }

}
