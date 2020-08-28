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
        /// <summary>
        /// 多张写真
        /// </summary>
        public List<string> Portraits { get; set; } = new List<string>();
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

    }
}
