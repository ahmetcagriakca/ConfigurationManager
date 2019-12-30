namespace ConfigurationManager.Api.Model
{
    public class ConfigurationEnvironment
    {
        public string ApplicationName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public int RefreshTimeInterval { get; set; }
        public bool IsValid()
        {
            return true;
        }

        public bool IsValid(out string message)
        {
            message = "";
            return true;
        }
    }
}
