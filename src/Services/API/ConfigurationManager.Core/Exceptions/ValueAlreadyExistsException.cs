using System;
using System.Collections.Generic;
using System.Text;

namespace ConfigurationManager.Core.Exceptions
{

    public class ValueAlreadyExistsException : Exception
    {
        public ValueAlreadyExistsException(string message) : base(message) { }
        public ValueAlreadyExistsException(string message, Exception innerException) : base(message, innerException) { }
    }
}
