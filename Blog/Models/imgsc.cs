using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blog.Models
{
    public class imgsc
    {
       
        public imgsc()
        {
          
        }

        public imgsc(Photocuss ph, Usersd us)
        {
            this.ph = ph;
            this.us = us;
        }

        public Photocuss ph { get; set; }
        public Usersd us { get; set; }
    }
}