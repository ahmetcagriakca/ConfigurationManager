namespace ConfigurationManager.Core
{
    public interface IConfigurationReader
    {
        T GetValue<T>(string key);
    }
}
