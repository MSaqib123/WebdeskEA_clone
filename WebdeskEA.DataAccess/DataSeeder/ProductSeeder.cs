using WebdeskEA.Models.DbModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebdeskEA.DataAccess.Data.DataSeeder
{
    public static class ProductSeeder
    {
        public static void Seed(ModelBuilder builder)
        {
            builder.Entity<Product>().HasData(
                new Product
                { 
                    Id=1,
                }
              );
        }
    }
}
