using Microsoft.AspNetCore.Mvc;
using Recollectable.Core.DTOs.Common;
using System.Collections.Generic;

namespace Recollectable.API.Controllers
{
    [Route("api")]
    public class HomeController : Controller
    {
        private IUrlHelper _urlHelper;

        public HomeController(IUrlHelper urlHelper)
        {
            _urlHelper = urlHelper;
        }

        [HttpGet(Name = "GetHome")]
        public IActionResult GetHome([FromHeader(Name = "Accept")] string mediaType)
        {
            if (mediaType == "application/json+hateoas")
            {
                var links = new List<LinkDto>();

                links.Add(new LinkDto(_urlHelper.Link("GetHome", 
                    new { }), "self", "GET"));

                links.Add(new LinkDto(_urlHelper.Link("GetUsers",
                    new { }), "users", "GET"));

                links.Add(new LinkDto(_urlHelper.Link("GetCollections",
                    new { }), "collections", "GET"));

                links.Add(new LinkDto(_urlHelper.Link("GetCoins",
                    new { }), "coins", "GET"));

                links.Add(new LinkDto(_urlHelper.Link("GetBanknotes",
                    new { }), "banknotes", "GET"));

                return Ok(links);
            }

            return NoContent();
        }
    }
}