using System.Collections;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi
{
    [Authorize]
    [Route("api/[controller]")]
    public class ClaimsController : Controller
    {
        // Return current claims
        [HttpGet]
        public IEnumerable Get()
            => User.Claims.ToDictionary(c => c.Type, c => c.Value);
    }
}