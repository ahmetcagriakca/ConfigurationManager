using System;
using System.Collections.Generic;
using System.Text;

namespace ConfigurationManager.Core
{
    public static class Extensions
    {
        public static long ToLong(this object value)
        {
            return Convert.ToInt64(value);
        }
        public static bool ToBoolean(this object value)
        {
            switch (value)
            {
                case "1":
                case 1:
                case true:
                case "true":
                    return true;
                case "0":
                case 0:
                case false:
                case "false":
                    return true;
                default:
                    throw new Exception("TypeNotFound");
            }
        }
    }
}
