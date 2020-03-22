using System;
using LiteDB;
using Microsoft.Extensions.Options;

namespace Zeus.Storage.LiteDb
{
    public class LiteDbProvider : IDisposable
    {
        private readonly Lazy<LiteDatabase> _dbFactory;

        public LiteDbProvider(IOptions<LiteDbOptions> optionsFactory)
        {
            _dbFactory = new Lazy<LiteDatabase>(() =>
            {
                var options = optionsFactory.Value;

                var mapper = options.ConfigureMapper != null 
                    ? new BsonMapper() 
                    : BsonMapper.Global;

                return new LiteDatabase(options.ConnectionString, mapper);
            });
        }

        public LiteDatabase GetDatabase()
        {
            return _dbFactory.Value;
        }


        public void Dispose()
        {
            if (_dbFactory.IsValueCreated)
                _dbFactory.Value.Dispose();
        }
    }
}