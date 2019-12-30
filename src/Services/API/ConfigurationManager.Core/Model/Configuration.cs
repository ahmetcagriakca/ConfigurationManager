namespace ConfigurationManager.Core.Model
{
    public class ApplicationConfiguration : Entity<long>
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public string ApplicationName { get; set; }
    }

    public class MongoSequence : Entity<string>
    {
        public object Sequence { get; set; }
    }
    public interface IEntity
    {
    }
    public interface IEntity<T>:IEntity
    {
        T Id { get; set; }
        bool IsActive { get; set; }
    }
    public class Entity<T> :IEntity<T>
    {
        public T Id { get; set; }
        public bool IsActive { get; set; }
    }
}
