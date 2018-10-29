using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recollectable.Core.Shared.Models;
using System.Collections.Generic;

namespace Recollectable.API.Controllers
{
    [Route("api")]
    //TODO Add Authorization [Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
        [HttpGet(Name = "GetHome")]
        public IActionResult GetHome([FromHeader(Name = "Accept")] string mediaType)
        {
            if (mediaType == "application/json+hateoas")
            {
                var links = new List<LinkDto>
                {
                    new LinkDto(Url.Link("GetHome",
                    new { }), "self", "GET"),

                    new LinkDto(Url.Link("GetUsers",
                    new { }), "users", "GET"),

                    new LinkDto(Url.Link("GetCollections",
                    new { }), "collections", "GET"),

                    new LinkDto(Url.Link("GetCoins",
                    new { }), "coins", "GET"),

                    new LinkDto(Url.Link("GetBanknotes",
                    new { }), "banknotes", "GET")
                };

                return Ok(links);
            }

            return NoContent();
        }
    }
}