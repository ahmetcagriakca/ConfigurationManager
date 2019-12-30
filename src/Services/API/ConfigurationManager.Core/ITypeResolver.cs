using MongoDB.Bson;
using System;

namespace ConfigurationManager.Core
{
    public interface ITypeResolver
    {
        object ValueConvertToType(string value, string type);
    }
    public class TypeResolver: ITypeResolver
    {
        public object ValueConvertToType(string value ,string type)
        {
            switch (type)
            {
                case "String":
                    return value.ToString();
                case "Int":
                    return Convert.ToInt32(value);
                case "Boolean":
                    return value.ToBoolean();
                case "Double":
                    return Convert.ToDouble(value);
                default:
                    return value;
            }
        }
    }
}
