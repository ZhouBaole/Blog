//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Blog.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Photograph
    {
        public int otoid { get; set; }
        public string otoname { get; set; }
        public System.DateTime otodate { get; set; }
        public int phid { get; set; }
        public string photos { get; set; }
    
        public virtual Phabum Phabum { get; set; }
    }
}