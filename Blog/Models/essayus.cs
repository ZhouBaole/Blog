using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blog.Models
{
    public class essayus
    {
        public essayus(Discuss dis, Usersd us)
        {
            this.dis = dis;
            this.us = us;
        }
        public essayus()
        {
           
        }

        public Discuss dis { get; set; }
        public Usersd us { get; set; }
    }
}