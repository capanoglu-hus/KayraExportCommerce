using Microsoft.EntityFrameworkCore;
using ProductService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductService.Persistence.Context
{
    public class ProductionServiceContext:DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;initial Catalog=KayraExport;integrated Security=true;Trusted_Connection=True;TrustServerCertificate=true");
        }
        public DbSet<Product> Products { get; set; }
        /* veritabanı bağlantı kurma 
         appsettings ekleyip düzenlenecek*/
    }
}
