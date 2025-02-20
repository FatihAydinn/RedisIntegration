using RedisIntegration_Redis.Interfaces;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisIntegration_Redis.Services
{
    public class RedisServices : IRedisServices
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly IDatabaseAsync _databaseAsync;
        public IDatabase Db { get; }

        public RedisServices(IConnectionMultiplexer connectionMultiplexer)
        {
            _connectionMultiplexer = connectionMultiplexer;
            _databaseAsync = _connectionMultiplexer.GetDatabase();
            Db = _connectionMultiplexer.GetDatabase();
        }

        public async Task ClearKeyAsync(string key)
        {
            await _databaseAsync.KeyDeleteAsync(key);
        }

        public async Task<string> GetKeyAsync(string key) 
        {
            return await _databaseAsync.StringGetAsync(key);
        }

        public async Task<bool> SetKeyAsync(string key, string val)
        {
            return await _databaseAsync.StringSetAsync(key, val);
        }

        public void ResetAllKeys()
        {
            var endPoints = _connectionMultiplexer.GetEndPoints(true);
            foreach (var endPoint in endPoints) {
                var server = _connectionMultiplexer.GetServer(endPoint);
                server.FlushAllDatabases();
            }
        }
    }
}
