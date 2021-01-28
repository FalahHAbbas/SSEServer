using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SSEDotnet;

namespace SSEServer.Controllers {
    [ApiController]
    [Route("api/[Controller]")]
    public class AuthUserController : ControllerBase {
        private static readonly List<User> Users = new(){
            new(){
                Id = 1,
                Name = "Admin",
                BirthDate = DateTime.Now
            }
        };

        private readonly SseService _sseService;

        public AuthUserController(SseService sseService){
            _sseService = sseService;
        }

        private string MethodType => HttpContext.Request.Method;


        // [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] User user){
            user.Id = Users.Count + 1;
            Users.Add(user);
            await _sseService.Send(
                _sseService.GetMethodPath(OnChange),
                user,
                _sseService.GetEvent(MethodType),
                ("Id", this.Id().ToString())
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
                _sseService.GetEvent(MethodType),
                ("Id", this.Id().ToString())
            );
            await _sseService.Send(
                _sseService.GetMethodPath(OnUpdate),
                user,
                _sseService.GetEvent(MethodType),
                ("id", id.ToString())
            );
            return Ok(user);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id){
            var result = Users.Find(user => user.Id == id);
            await _sseService.Send(
                _sseService.GetMethodPath(OnChange),
                result,
                _sseService.GetEvent(MethodType),
                ("Id", this.Id().ToString())
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
            await _sseService.Create(HttpContext,
                ("Id", this.Id().ToString()),
                ("", "")
            );
        }


        [HttpGet("OnUpdate")]
        public async Task OnUpdate(){
            var id = this.GetHeader("data");
            await _sseService.Create(HttpContext,
                ("id", id)
            );
        }


        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] User user){
            var result = Users.Find(user1 => user1.Id == user.Id);
            return Ok(new{
                Token = GenerateToken(result)
            });
        }

        private string GenerateToken(User user){
            return new JwtSecurityTokenHandler().WriteToken(
                new JwtSecurityToken(
                    claims: new[]{
                        new Claim("Id", user.Id.ToString()),
                        new Claim(JwtRegisteredClaimNames.Sub, user.Name),
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                    },
                    expires: DateTime.UtcNow.AddDays(30),
                    notBefore: DateTime.UtcNow, audience: "Audience", issuer: "Issuer",
                    signingCredentials: new SigningCredentials(
                        new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes("Hlkjds0-324mf34pojf-34r34fwlknef0943")),
                        SecurityAlgorithms.HmacSha256)
                )
            );
        }
    }
}