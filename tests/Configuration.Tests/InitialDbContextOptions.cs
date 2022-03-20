using ConfigurationCase.DAL;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configuration.Tests
{
    public class InitialDbContextOptions
    {
        protected DbContextOptions<ConfigurationDbContext> _contextOptions { get; private set; }

        public void SetContextOptions(DbContextOptions<ConfigurationDbContext> contextOptions)
        {
            _contextOptions = contextOptions;
            Seed();
        }

        public void Seed()
        {
            using (ConfigurationDbContext context = new ConfigurationDbContext(_contextOptions))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                context.Configuration.Add(new ConfigurationCase.Core.Entities.Configuration { ID = 1, Name = "SiteName1", ApplicationName = "SERVICE-A-TEST", Type = Core.Enums.ConfigurationTypeEnum.String, IsActive = true, Value = "boyner.com" });
                context.Configuration.Add(new ConfigurationCase.Core.Entities.Configuration { ID = 2, Name = "IsBasketEnabled", ApplicationName = "SERVICE-A-TEST", Type = Core.Enums.ConfigurationTypeEnum.Boolean, IsActive = true, Value = "1" });
                context.Configuration.Add(new ConfigurationCase.Core.Entities.Configuration { ID = 3, Name = "MaxItemCount", ApplicationName = "SERVICE-A-TEST", Type = Core.Enums.ConfigurationTypeEnum.Int, IsActive = true, Value = "50" });
                context.SaveChanges();
            }
        }
    }
}
