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
        string movieRootPath = Directory.GetCurrentDirectory() + @"\wwwroot\ftp\";


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
        private List<DirectoryInfo> GetActressDirs(string dir1)
        {
            DirectoryInfo dir = new DirectoryInfo(dir1);
            var dirs = dir.GetDirectories();
            return dirs.ToList();
        }
        private async Task<List<v_Movie>> GetListInternal()
        {

            //播放量字典
            Dictionary<string, int> viewCountsDict = this.GetViewCountDict();
            string ApiHost = _configuration.GetValue<string>("ApiHost");
            string apiHostExt = ApiHost + @"/ftp/";

            List<v_Movie> LsMovies = new List<v_Movie>();

            var actressDirs = this.GetActressDirs(movieRootPath);
            foreach (var actressItem in actressDirs) //每女优
            {
                v_Actress actress = new v_Actress();
                actress.FullName = actressItem.Name;
                var actressFiles = actressItem.GetFiles();// 获取女优图片
                foreach (var photoItem in actressFiles)
                {
                    string picRelativePath = photoItem.FullName.Replace(movieRootPath, "");
                    string path = apiHostExt + picRelativePath?.Replace("\\", "/");
                    actress.Portraits.Add(path);
                    if (string.IsNullOrEmpty(actress.Cover))
                    {
                        actress.Cover = path;
                    }
                }
                var fanHaoDirs = actressItem.GetDirectories(); //所有番号 文件夹
                foreach (var fanHaoItem in fanHaoDirs)
                {
                    var fanHaoFiles = fanHaoItem.GetFiles();
                    string FanHao = fanHaoItem.Name.Replace("-C", "");
                    v_Movie movie = new v_Movie();
                    foreach (var item in fanHaoFiles)
                    {
                        var ext = item.Extension;
                        if(ext == ".mp4" || ext == ".webm") //视频
                        {
                            var Source = apiHostExt + item.FullName.Replace(movieRootPath, "")?.Replace("\\", "/");
                            movie.Sources.Add(Source);
                            if (!LsMovies.Contains(movie))
                            {
                                movie.Id = LsMovies.Count + 1;
                                
                                movie.FanHao = FanHao;

                                movie.Date = item.CreationTime.ToString("yyyy-MM-dd hh:mm");
                                movie.CreationTime = item.CreationTime;
                                movie.ViewCount = viewCountsDict.Any(x => x.Key == FanHao) ? viewCountsDict[FanHao] : 0;
                                movie.Actress = actress;

                                LsMovies.Add(movie);
                            }
                        }
                        if (ext == ".jpg" || ext == ".png") //图片
                        {
                            string picRelativePath = item.FullName.Replace(movieRootPath, "")?.Replace("\\", "/");

                            if (item.Name.Contains(FanHao))
                            {
                                movie.Title = Path.GetFileNameWithoutExtension(item.Name);
                                movie.Cover = apiHostExt + picRelativePath;
                            }
                            else
                            {
                                movie.Preview.Add(apiHostExt + picRelativePath);
                            }
                        }
                    }
                }
            }
            //默认值
            foreach (var item in LsMovies)
            {
                if (!item.Cover.Contains(".jpg")) //视频不包含图片添加默认图片
                {
                    item.Cover = "../images/default.jpg";
                }
            }

            _memoryCache.Set("movieTempList", LsMovies, new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(30)));
            return LsMovies;
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
        public async Task<List<v_Movie>> GetList(int pageIndex, int pageSize, string sort)
        {
            //List<v_Movie> list = new List<v_Movie>();
            var _cache = _memoryCache.Get<List<v_Movie>>("movieTempList");
            if (_cache != null && _cache.Count > 0)
            {
                Task.Run(() => { this.GetListInternal(); });
                return _cache;
            }

            List<v_Movie> list = await this.GetListInternal();

            //排序
            if (sort == SortType.CreateTime.ToString())
            {
                list = list.OrderByDescending(x => x.CreationTime).ToList();
            }
            else if(sort == SortType.ViewCount.ToString())
            {
                list = list.OrderByDescending(y => y.ViewCount).ToList();
            }
            return list;
        }


        public async Task<v_Movie> DetailAsync(int id)
        {
            v_Movie movie = new v_Movie();
            var _cache = _memoryCache.Get<List<v_Movie>>("movieTempList");
            Dictionary<string, int> viewCountsDict = this.GetViewCountDict();
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
                movie = detail;
            }
            return movie;
        }
        public async Task<List<v_Actress>> Actresses()
        {
            List<v_Actress> list = new List<v_Actress>();
            var _cache = _memoryCache.Get<List<v_Actress>>("ActressTempList");
            if (_cache != null && _cache.Count > 0)
            {
                Task.Run(() => { this.GetActressesInternal(); });
                list = _cache;
                return _cache;
            }
            return await this.GetActressesInternal(); ;

        }
        public async Task<List<v_Actress>> GetActressesInternal()
        {
            string ApiHost = _configuration.GetValue<string>("ApiHost");
            string apiHostExt = ApiHost + @"/ftp/";
            List<v_Actress> lsActress = new List<v_Actress>();

            var actressDirs = this.GetActressDirs(movieRootPath);
            foreach (var actressItem in actressDirs) //每女优
            {
                v_Actress actress = new v_Actress();
                actress.FullName = actressItem.Name;
                var actressFiles = actressItem.GetFiles();// 获取女优图片
                foreach (var photoItem in actressFiles)
                {
                    string picRelativePath = photoItem.FullName.Replace(movieRootPath, "");
                    string path = apiHostExt + picRelativePath?.Replace("\\", "/");
                    actress.Portraits.Add(path);
                    if (string.IsNullOrEmpty(actress.Cover))
                    {
                        actress.Cover = path;
                    }
                }
                lsActress.Add(actress);
            }
            return lsActress;
        }


    }

}
