using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RedisIntegration_Redis.Interfaces;

namespace API_Integration_with_Redis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RedisController : ControllerBase
    {
        private readonly IRedisServices _redisServices;
       
        public RedisController(IRedisServices redisServices) {
            _redisServices = redisServices;
        }

        [HttpPost("cache/{key}")]
        public async Task<IActionResult> Get(string key)
        {
            return Ok(await _redisServices.GetKeyAsync(key));
        }

        [HttpPost("cache/set")]
        public async Task<IActionResult> Set(string key, string val)
        {
            await _redisServices.SetKeyAsync(key,val);
            return Ok();
        }

        [HttpPost("cache/delete/{key}")]
        public async Task<IActionResult> Reset(string key)
        {
            await _redisServices.ClearKeyAsync(key);
            return Ok();
        }
    }
}
