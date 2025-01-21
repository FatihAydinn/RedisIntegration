using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisIntegration_Redis.Interfaces
{
    public interface IRedisServices
    {
        Task ClearKeyAsync(string key);
        Task<string> GetKeyAsync(string key);
        Task<bool> SetKeyAsync(string key, string val);
        void ResetAllKeys();
    }
}
