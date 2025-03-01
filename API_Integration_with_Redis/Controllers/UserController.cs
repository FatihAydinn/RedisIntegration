using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RedisIntegration_Redis.Interfaces;
using RedisIntegration_Redis.Services;
using StackExchange.Redis;
using System.Text.Json;

namespace API_Integration_with_Redis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IRedisServices _redisService;

        public UserController(IRedisServices redisService)
        {
            _redisService = redisService;
        }

        [HttpPost]
        public IActionResult CreateUser([FromBody] User user)
        {
            var key = $"user:{user.Id}";
            var json = JsonSerializer.Serialize(user);
            _redisService.Db.StringSet(key, json);
            return Ok(new { Message = "User created", UserId = user.Id });
        }

        [HttpGet("{id}")]
        public IActionResult GetUser(int id)
        {
            var key = $"user:{id}";
            var userData = _redisService.Db.StringGet(key);

            if (userData.IsNullOrEmpty)
                return NotFound(new { Message = "User not found" });

            var user = JsonSerializer.Deserialize<User>(userData);
            return Ok(user);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateUser(int id, [FromBody] User updatedUser)
        {
            var key = $"user:{id}";

            if (!_redisService.Db.KeyExists(key))
                return NotFound(new { Message = "User not found" });

            var json = JsonSerializer.Serialize(updatedUser);
            _redisService.Db.StringSet(key, json);
            return Ok(new { Message = "User updated" });
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            var key = $"user:{id}";

            if (!_redisService.Db.KeyExists(key))
                return NotFound(new { Message = "User not found" });

            _redisService.Db.KeyDelete(key);
            return Ok(new { Message = "User deleted" });
        }

        [HttpPost("PostWithHash")]
        public IActionResult CreateUserWithHash2([FromBody] User user)
        {
            var key = $"user:{user.Id}";

            var userEntries = new HashEntry[]
            {
                new ("Name", user.Name),
                new ("Email", user.Email),
                new ("Address", user.Address),
                new ("Age", user.Age.ToString())
            };

            _redisService.Db.HashSet(key, userEntries);
            _redisService.Db.HashSet("all_users", key, JsonSerializer.Serialize(user));

            return Ok(new { Message = "User created", UserId = user.Id });
        }

        [HttpGet("GetwithHash/{id}")]
        public IActionResult GetUserWithHast(int id)
        {
            var key = $"user:{id}";
            var userData = _redisService.Db.HashGetAll(key);

            if (userData.Length == 0)
                return NotFound(new { Message = "User not found" });

            var user = new User
            {
                Id = id,
                Name = userData.FirstOrDefault(x => x.Name == "Name").Value,
                Email = userData.FirstOrDefault(x => x.Name == "Email").Value,
                Address = userData.FirstOrDefault(x => x.Name == "Address").Value,
                Age = int.Parse(userData.FirstOrDefault(x => x.Name == "Age").Value)
            };

            return Ok(user);
        }

        [HttpGet("allUsersWithHash")]
        public IActionResult GetAllUsers()
        {
            var allUsers = _redisService.Db.HashGetAll("all_users");
            var users = new List<User>();

            foreach (var userEntry in allUsers)
            {
                var userJson = userEntry.Value;
                var user = JsonSerializer.Deserialize<User>(userJson);
                users.Add(user);
            }

            return Ok(users);
        }
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public int Age { get; set; }
    }
}
