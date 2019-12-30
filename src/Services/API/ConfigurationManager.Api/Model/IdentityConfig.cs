using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfigurationManager.Api.Model
{
    public class IdentityConfig
    {
        public string Authority { get; set; }
        public string Audience { get; set; }
    }
}
