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
            optionsBuilder.UseSqlite("Data Source=/home/bilimedal/BiliMedal.db");
            //optionsBuilder.UseSqlite("Data Source=./BiliMedal.db");
        }
        /// <summary>
        /// 设置sqlite数据库journal_mode为WAL
        /// </summary>
        public void SetWALMode()
        {
            using (var connection = this.Database.GetDbConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "PRAGMA journal_mode=WAL;";
                    string result = command.ExecuteScalar() as string;
                    Console.WriteLine("切换journal_mode结果：" + result);
                }
            }
        }
    }
}
