using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configuration.Core.Events
{
    public class RequestConfigurationEvent
    {
        public string ApplicationName { get; set; }
        public string ConnectionString { get; set; }
        public int RefreshTimerIntervalInMs { get; set; }
    }
}
