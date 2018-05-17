using System;
using System.Collections.Generic;
using System.Text;

namespace LittleSexy.Model
{
    public abstract class DBEntityBase
    {
        public long Id { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreateDateTime { get; set; }
        public string Remark { get; set; }
    }
}
