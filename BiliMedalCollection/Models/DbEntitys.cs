using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BiliMedalCollection.Models
{
    public class DbEntitys: DbContext
    {
        //public DbEntitys(DbContextOptions<DbEntitys> options)
        //    : base(options) { }

        public DbSet<Medal> Medals { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=BiliMedal.db");
        }
    }
}
