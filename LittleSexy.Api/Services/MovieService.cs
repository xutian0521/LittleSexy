using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;
using System.Data;
using LittleSexy.Api.Util;
using LittleSexy.Api.Models;

namespace LittleSexy.Api.Services
{

    public class MovieService
    {
        public static IMemoryCache _memoryCache = new MemoryCache(new MemoryCacheOptions());
        //todo:业务层改用多个实例
        public IConfiguration _configuration { get; }
        public MovieService(IServiceProvider service,IConfiguration configuration)
        {
            _configuration = configuration;
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
        private async Task<ApiResult> GetListInternal()
        {
            ApiResult result = new ApiResult();
            //播放量字典
            Dictionary<string, int> viewCountsDict = this.GetViewCountDict();
            string movieRootPath = Directory.GetCurrentDirectory() + @"\wwwroot\ftp\";
            List<List<FileInfo>> fileList = new List<List<FileInfo>>();
            if (Directory.Exists(movieRootPath))
            {
                this.GetAllDir(movieRootPath, ref fileList); //递归查询文件目录
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
                    if (ext == ".mp4" || ext == ".webm") //视频
                    {
                        movie.Title = item.Name;
                        movie.FanHao = Path.GetFileNameWithoutExtension(item.Name.Replace("-C", ""));
                        movie.Source = item.FullName.Replace(movieRootPath, "");

                        movie.CreationTime = item.CreationTime;
                    }
                }

                foreach (var item in itemOnes)
                {
                    var ext = Path.GetExtension(item.FullName);
                    if (ext == ".jpg" || ext == ".png") //图片
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
                if (!movie.Cover.Contains(".jpg")) //视频不包含图片添加默认图片
                {
                    movie.Cover = "../images/default.jpg";
                }
                movieLists.Add(movie);
            }
            string ApiHost = _configuration.GetValue<string>("ApiHost");
            List<v_Movie> lsMovie = new List<v_Movie>();
            foreach (var item in movieLists.OrderByDescending(x => x.CreationTime))
            {
                v_Movie model = new v_Movie();
                model.Id = lsMovie.Count + 1;
                model.Title = item.Title;
                model.FanHao = item.FanHao;
                model.Cover = ApiHost + @"/ftp/" + item.Cover?.Replace("\\", "/");
                model.LinkUrl = "{path:'movie/detail', query: { id: " + model.Id + " }}";
                model.Source = ApiHost + @"/ftp/" + item.Source;
                model.Date = item.CreationTime.ToString("yyyy-MM-dd hh:mm");
                model.CreationTime = item.CreationTime;
                model.ViewCount = viewCountsDict.Any(x => x.Key == item.FanHao) ? viewCountsDict[item.FanHao] : 0;
                lsMovie.Add(model);
            }
            result.Content = lsMovie;
            _memoryCache.Set("movieTempList", lsMovie, new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(30)));
            return result;
        }
        private Dictionary<string, int> GetViewCountDict()
        {
            Dictionary<string, int> viewCountsDict = new Dictionary<string, int>();
            var dt = SQLiteHelper.Query("SELECT * FROM Dict;");
            if (dt.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow item in dt.Tables[0].Rows)
                {
                    string fanHao = item["Datakey"].ToString();
                    int count = int.Parse(item["DataValue"].ToString());
                    viewCountsDict.Add(fanHao, count);
                }
            }
            return viewCountsDict;
        }
        public async Task<ApiResult> GetList(int pageIndex, int pageSize, string sort)
        {
            List<v_Movie> list = new List<v_Movie>();
            var _cache = _memoryCache.Get<List<v_Movie>>("movieTempList");
            if (_cache != null && _cache.Count > 0)
            {
                Task.Run(() => { this.GetListInternal(); });
                list = _cache;
                return new ApiResult() { Content = _cache };
            }

            
            var result= await this.GetListInternal();
            list = result.Content;

            //排序
            if (sort == SortType.CreateTime.ToString())
            {
                list = list.OrderByDescending(x => x.CreationTime).ToList();
            }
            else if(sort == SortType.ViewCount.ToString())
            {
                list = list.OrderByDescending(y => y.ViewCount).ToList();
            }
            return result;

        }


        public async Task<ApiResult> DetailAsync(int id)
        {
            var _cache = _memoryCache.Get<List<v_Movie>>("movieTempList");
            Dictionary<string, int> viewCountsDict = this.GetViewCountDict();
            ApiResult result = new ApiResult();
            List<v_Movie> movieLists = new List<v_Movie>();

            if (_cache != null && _cache.Count > 0)
            {
                var detail = _cache.Where(x => x.Id == id).FirstOrDefault();
                int count = 0;
                var dt = SQLiteHelper.Query($"SELECT * FROM Dict  WHERE DataKey='{detail.FanHao}'");
                if (dt.Tables[0].Rows.Count > 0)
                {
                    var dr = dt.Tables[0].Rows[0];
                    count =int.Parse( dr["DataValue"].ToString());
                    int _id = int.Parse(dr["Id"].ToString());
                    int row = SQLiteHelper.ExecuteNonQuery($"UPDATE Dict SET DataValue = '{ ++count}' WHERE Id = '{_id}'; ");
                }
                else
                {
                    int row = SQLiteHelper.ExecuteNonQuery($"INSERT INTO Dict ( DataKey,DataValue) VALUES ('{detail.FanHao}','1');");
                }
                detail.ViewCount = viewCountsDict.Any(x => x.Key == detail.FanHao) ? viewCountsDict[detail.FanHao] : 0;
                result.Content = detail;
            }
            return result;
        }
    }

}
