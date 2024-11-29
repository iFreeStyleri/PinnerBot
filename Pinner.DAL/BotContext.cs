using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Pinner.DAL.Entities;

namespace Pinner.DAL
{
    public class BotContext : DbContext
    {
        public DbSet<Tag> Tags => Set<Tag>();
        public DbSet<Topic> Topics => Set<Topic>();

        public BotContext(DbContextOptions opt) : base(opt)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        }
    }
}
