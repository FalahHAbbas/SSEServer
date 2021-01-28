using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSEDotnet;

namespace SSEServer.Controllers {
    [ApiController]
    [Route("api/[Controller]")]
    public class UserController : ControllerBase {
        private static readonly List<User> Users = new();
        private readonly SseService _sseService;

        public UserController(SseService sseService){
            _sseService = sseService;
        }

        private string MethodType => HttpContext.Request.Method;


        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] User user){
            user.Id = Users.Count + 1;
            Users.Add(user);
            await _sseService.Send(
                _sseService.GetMethodPath(OnChange),
                user,
                _sseService.GetEvent(MethodType)
            );
            return Ok(user);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] User user){
            var result = Users.Find(u => u.Id == id);
            user.Id = id;
            Users.Insert(Users.IndexOf(result), user);
            await _sseService.Send(
                _sseService.GetMethodPath(OnChange),
                user,
                _sseService.GetEvent(MethodType)
            );
            return Ok(user);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id){
            var result = Users.Find(user => user.Id == id);
            await _sseService.Send(
                _sseService.GetMethodPath(OnChange),
                result,
                _sseService.GetEvent(MethodType)
            );
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long id){
            var user = Users.Find(user1 => user1.Id == id);
            return Ok(user);
        }

        [HttpGet("All/{pageNumber}")]
        public async Task<IActionResult> GetAll(int pageNumber, int pageSize = 10){
            var users = Users.Skip(pageSize * (pageNumber - 1))
                .Take(pageSize).ToList();
            return Ok(users);
        }

        [HttpGet("OnChange")]
        public async Task OnChange(){
            await _sseService.Create(HttpContext);
        }
    }
}