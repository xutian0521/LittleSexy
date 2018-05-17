using System;
using System.Collections.Generic;
using System.Text;

namespace LittleSexy.Model.DBModel
{
    public class t_Page : DBEntityBase
    {

        public string Title { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public bool HasBanner { get; set; }
    }
}
