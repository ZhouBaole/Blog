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
    
    public partial class Melike
    {
        public int mlid { get; set; }
        public int usid { get; set; }
        public System.DateTime medate { get; set; }
        public int meid { get; set; }
    
        public virtual Message Message { get; set; }
        public virtual Usersd Usersd { get; set; }
    }
}