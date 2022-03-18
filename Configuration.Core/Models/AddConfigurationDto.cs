using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configuration.Core.Models
{
    public class AddConfigurationDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        [Required]
        [StringLength(100)]
        public string Type { get; set; } // TODO: Enum şeklinde çağrılabilir
        [Required]
        [StringLength(300)]
        public string Value { get; set; }
        public bool IsActive { get; set; }

    }
}
