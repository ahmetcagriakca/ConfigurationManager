using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfigurationManager.Api.Utility
{
    public class BaseServiceResponse
    {
        public BaseServiceResponse()
        {
            ResponseTime = DateTime.Now;
            IsSuccess = true;
        }

        public bool IsSuccess { get; set; }
        public dynamic ResultValue { get; set; }
        public DateTime ResponseTime { get; set; }
    }
}
