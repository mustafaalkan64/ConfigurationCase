using Configuration.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigurationCase.Core.Entities
{
    public class Configuration
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public ConfigurationTypeEnum Type { get; set; }
        public string Value { get; set; }
        public bool IsActive { get; set; }
        public string ApplicationName { get; set; }
    }
}
