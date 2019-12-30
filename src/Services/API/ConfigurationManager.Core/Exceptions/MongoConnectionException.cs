using System;
using System.Collections.Generic;
using System.Text;

namespace ConfigurationManager.Core
{

    public class MongoConnectionException : Exception
    {
        public MongoConnectionException(string message) : base(message) { }
        public MongoConnectionException(string message, Exception innerException) : base(message, innerException) { }
    }
}
