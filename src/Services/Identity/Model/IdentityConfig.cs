using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfigurationManager.Api.Model
{
    public class IdentityConfig
    {
        public string CertificatePath { get; set; }
        public string CertificatePassword { get; set; }
    }
}
