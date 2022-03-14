using ConfigurationCase.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigurationCase.DAL
{
    public class ConfigurationDbContext : DbContext
    {
        //public ConfigurationDbContext(DbContextOptions<ConfigurationDbContext> options) : base(options)
        //{
        //}

        public ConfigurationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Configuration> Configuration { get; set; }
    }
}
