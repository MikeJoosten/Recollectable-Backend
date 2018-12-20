using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recollectable.Core.Shared.Models;
using System.Collections.Generic;

namespace Recollectable.API.Controllers
{
    [ApiVersion("1.0")]
    [Route("api")]
    //TODO Add Authorization [Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
        /// <summary>
        /// Retrieves the main HATEOAS links
        /// </summary>
        /// <param name="mediaType"></param>
        /// <response code="200">Returns the HATEOAS links</response>
        /// <response code="204">No application/json+hateoas media type detected</response>
        [HttpGet(Name = "GetHome")]
        [Produces("application/json", "application/json+hateoas", "application/xml")]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
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