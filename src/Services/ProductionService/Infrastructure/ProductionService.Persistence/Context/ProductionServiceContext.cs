using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ProductionService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductionService.Persistence.Context
{
    public class ProductionServiceContext:DbContext
    {
        public ProductionServiceContext(DbContextOptions<ProductionServiceContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
          
        }
        public DbSet<Product> Products { get; set; }
        /* veritabanı bağlantı kurma 
         appsettings ekleyip düzenlenecek*/
    }
}
