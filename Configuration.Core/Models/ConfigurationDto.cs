using Configuration.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configuration.Core.Models
{
    public class ConfigurationDto
    {
        private string typeName;
        public int ID { get; set; }
        public string Name { get; set; }
        public ConfigurationTypeEnum Type { get; set; }
        public string TypeName {
            get
            {
                return typeName;
            }

            set
            {
                typeName = Type.ToString();
            }
           
        }
        public string Value { get; set; }
        public bool IsActive { get; set; }
        public string ApplicationName { get; set; }
    }
}
