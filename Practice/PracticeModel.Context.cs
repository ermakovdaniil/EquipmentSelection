﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Practice
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class practiceEntities : DbContext
    {
        public practiceEntities()
            : base("name=practiceEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<characteristics> characteristics { get; set; }
        public virtual DbSet<engineering> engineering { get; set; }
        public virtual DbSet<engineeringtype> engineeringtype { get; set; }
        public virtual DbSet<grindingmethod> grindingmethod { get; set; }
        public virtual DbSet<material> material { get; set; }
        public virtual DbSet<process> process { get; set; }
    }
}
