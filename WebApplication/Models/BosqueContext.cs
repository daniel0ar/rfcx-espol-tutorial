using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace WebApplication.Models
{
    public class BosqueContext : DbContext
    {
        public BosqueContext(DbContextOptions<BosqueContext> options) : base(options)
        {}
        
        public DbSet<Alert> Alerts { get; set; }
        public DbSet<Audio> Audios { get; set; }
        public DbSet<Station> Stations { get; set; } 
        public DbSet<Sensor> Sensors { get; set; }
        public DbSet<Label> Label { get; set; }


        /** 
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=bosque.db");
        }
        */

    }
}