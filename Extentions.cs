using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSEDotnet;

namespace SSEServer {
    public static class Extentions {

        public static async Task Create(this SseService service, HttpContext context,
            params (string n, string v)[] props){
            await SseEmitter.Create(context, service, props);
        }

        public static long Id(this ControllerBase controllerBase){
            return long.Parse(GetClaim(controllerBase, nameof(User.Id)));
        }

        public static string GetClaim(this ControllerBase controllerBase, string claimName){
            return (controllerBase.User.Identity as ClaimsIdentity)?.Claims
                .FirstOrDefault(c =>
                    string.Equals(c.Type, claimName, StringComparison.CurrentCultureIgnoreCase))
                ?.Value;
        }

        public static string GetHeader(this ControllerBase controllerBase, string headerName){
            return controllerBase.Request.Headers[headerName];
        }
    }
}