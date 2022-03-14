using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigurationCase.Core.Models
{
    public class RedisServerConfig
    {

        #region Props

        public string PrivateKey { get; set; }
        public string RedisEndPoint { get; set; }
        public int RedisPort { get; set; }
        public string RedisPassword { get; set; }
        public int RedisTimeout { get; set; }
        public bool RedisSsl { get; set; }

        #endregion
        public RedisServerConfig()
        {
            PrivateKey = "+IfWDqELQ2zBE6sI4D4ncSMBvTagujpBx0b5uieu8jI=æXB2DUtejEziVgiBPkRSahA==";
        }
    }
}
