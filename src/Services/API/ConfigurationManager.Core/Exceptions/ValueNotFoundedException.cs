using System;
using System.Collections.Generic;
using System.Text;

namespace ConfigurationManager.Core
{

    public class ValueNotFoundedException : Exception
    {
        public ValueNotFoundedException(string message) : base(message) { }
        public ValueNotFoundedException(string message, Exception innerException) : base(message, innerException) { }
    }
}
