using ConfigurationCase.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigurationCase.CommonService
{
    public class ConfigurationDbContext : DbContext
    {
        //public ConfigurationDbContext(DbContextOptions options) : base(options)
        //{
        //}

        public ConfigurationDbContext(DbContextOptions<ConfigurationDbContext> options) : base(options)
        {
        }

        public DbSet<ConfigurationTb> Configuration { get; set; }
    }
}
