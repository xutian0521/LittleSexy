using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LittleSexy.Api.Models
{
    public class v_Actress
    {
        public int Id { get; set; }
        /// <summary>
        /// 封面
        /// </summary>
        public string Cover { get; set; }
        public string Background { get; set; }
        /// <summary>
        /// 多张写真
        /// </summary>
        public List<string> Portraits { get; set; } = new List<string>();
        /// <summary>
        /// 电影作品数量
        /// </summary>
        public int MovieCount { get; set; }
        /// <summary>
        /// 电影作品总观看次数
        /// </summary>
        public int TotalViewCount { get; set; }
        /// <summary>
        /// 日文名
        /// </summary>
        public string JapaneseName { get; set; }
        /// <summary>
        /// 中文名
        /// </summary>
        public string ChineseName { get; set; }

        public string FullName { get; set; }
        /// <summary>
        /// 出生日期
        /// </summary>
        public DateTime Birthdate { get; set; }
        /// <summary>
        /// 出生地
        /// </summary>
        public string Birthplace { get; set; }
        /// <summary>
        /// 罩杯
        /// </summary>
        public string BrassiereCup { get; set; }
        /// <summary>
        /// 三维
        /// </summary>
        public string Measurements { get; set; }
        /// <summary>
        /// 身高
        /// </summary>
        public string Height { get; set; }
        /// <summary>
        /// 血型
        /// </summary>
        public string BloodType { get; set; }
        /// <summary>
        /// 兴趣爱好
        /// </summary>
        public string Interest { get; set; }
        /// <summary>
        /// 浏览次数
        /// </summary>
        public int ViewCount { get; set; }
        public DateTime CreationTime { get; set; }
        /// <summary>
        /// 是否喜欢
        /// </summary>
        public int IsLiked { get; set; }

    }
}
