using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigurationCase.Core.CustomExceptions
{
    public class RedisNotAvailableException : Exception
    {
        public string _errorCode = "431";
        public override string Message
        {
            get
            {
                return "Redis is not available.";
            }
        }

        public string ErrorCode
        {
            get
            {
                return _errorCode;
            }
        }
    }
}
