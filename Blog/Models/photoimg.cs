using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blog.Models
{
    public class photoimg
    {
        public photoimg(Phabum phabums, List<Photograph> pimg)
        {
            this.phabums = phabums;
            this.pimg = pimg;
        }
        public photoimg()
        {
           
        }
        public Phabum phabums { get; set; }
        public List<Photograph>pimg { get; set; }

    }
}