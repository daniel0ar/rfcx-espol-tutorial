using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace WebApplication.Helpers
{
    public class PreRequestModifications
    {

        private readonly RequestDelegate _next;

        public PreRequestModifications(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Cookies["token"];
            if (token != null)
            {
                context.Request.Headers["Authorization"] = "Bearer " + context.Request.Cookies["token"];
                
            }
            await _next.Invoke(context);
        }
    }

    
}