﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class BlognEntities : DbContext
    {
        public BlognEntities()
            : base("name=BlognEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Discuss> Discuss { get; set; }
        public virtual DbSet<Dislike> Dislike { get; set; }
        public virtual DbSet<Eslike> Eslike { get; set; }
        public virtual DbSet<Essay> Essay { get; set; }
        public virtual DbSet<Melike> Melike { get; set; }
        public virtual DbSet<Message> Message { get; set; }
        public virtual DbSet<Pclike> Pclike { get; set; }
        public virtual DbSet<Phabum> Phabum { get; set; }
        public virtual DbSet<Phlike> Phlike { get; set; }
        public virtual DbSet<Photocuss> Photocuss { get; set; }
        public virtual DbSet<Photograph> Photograph { get; set; }
        public virtual DbSet<Usersd> Usersd { get; set; }
    }
}