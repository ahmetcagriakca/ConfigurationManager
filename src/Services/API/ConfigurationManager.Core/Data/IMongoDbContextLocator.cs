using System;
using System.Collections.Generic;
using System.Text;

namespace ConfigurationManager.Core.Data
{
    /// <summary>
    /// MongoDbContext abstraction
    /// </summary>
    public interface IMongoDbContextLocator
    {
        MongoDbContext Current { get; }
    }
    public class DbContextLocator : IMongoDbContextLocator
    {
        private readonly MongoDbContext _dbContext;
        public DbContextLocator(MongoDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        MongoDbContext IMongoDbContextLocator.Current
        {
            get
            {
                return _dbContext;
            }
        }
    }
}
