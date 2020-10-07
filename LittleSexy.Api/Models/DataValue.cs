using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LittleSexy.Api.Models
{
    public class DataValue
    {
        public int count {get;set;} 
        public int isLiked{get;set;} 
        public DateTime LastAccessTime {get;set;}
        public int Height {get;set;}
        public int Width {get;set;}
        public string TotalTime {get;set;}
    }
}
