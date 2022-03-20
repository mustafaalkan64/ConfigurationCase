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

                //context.Configuration.Add(new Reports { Id = report1Id, CreatedDate = DateTime.Now, ReportStatus = Core.ReportStatusEnum.Preparing, RequestedDate = DateTime.Now });
                //context.Reports.Add(new Reports { Id = report2Id, CreatedDate = DateTime.Now, ReportStatus = Core.ReportStatusEnum.Preparing, RequestedDate = DateTime.Now });
                //context.Reports.Add(new Reports { Id = report3Id, CreatedDate = DateTime.Now, ReportStatus = Core.ReportStatusEnum.Preparing, RequestedDate = DateTime.Now });
                //context.SaveChanges();

                //context.ReportDetail.Add(new ReportDetail() { Id = Guid.NewGuid(), CreatedDate = DateTime.Now, Lat = 35, Long = 27, RegisteredPersonCount = 3, RegisteredPhoneCount = 3, ReportId = report1Id });
                //context.ReportDetail.Add(new ReportDetail() { Id = Guid.NewGuid(), CreatedDate = DateTime.Now, Lat = 35, Long = 24, RegisteredPersonCount = 2, RegisteredPhoneCount = 2, ReportId = report2Id });
                //context.ReportDetail.Add(new ReportDetail() { Id = Guid.NewGuid(), CreatedDate = DateTime.Now, Lat = 42, Long = 28, RegisteredPersonCount = 3, RegisteredPhoneCount = 4, ReportId = report3Id });

                context.SaveChanges();
            }
        }
    }
}
