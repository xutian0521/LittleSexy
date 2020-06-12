using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LittleSexy.Model.DBModel;
using System.IO;
using System.Diagnostics;

namespace LittleSexy.DAL
{
    public class MovieDAL_Text
    {
        public async Task<int> InsertMovieListAsync(List<t_Movie> movies)
        {
            using (StreamWriter sw = new StreamWriter(@"2.txt", false, Encoding.UTF8))
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                int i = 0;
                foreach (var item in movies)
                {
                    i++;//记录读取的行数
                    string rowText = $@"{item.Title}, {item.FanHao}, {item.Source}, {item.Cover}, {item.Details}, {item.CreationTime}";
                    await sw.WriteLineAsync(rowText);
                }

                stopwatch.Stop();
                Console.WriteLine(stopwatch.Elapsed);
                Console.WriteLine(i);
                return i;
            }

        }
    }
}
