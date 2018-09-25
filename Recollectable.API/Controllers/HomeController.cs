using Microsoft.AspNetCore.Mvc;
using Recollectable.Core.Interfaces.Services;
using Recollectable.Core.Shared.DTOs;
using System.Collections.Generic;

namespace Recollectable.API.Controllers
{
    [Route("api")]
    public class HomeController : Controller
    {
        public readonly IControllerService _controllerService;

        public HomeController(IControllerService controllerService)
        {
            _controllerService = controllerService;
        }

        [HttpGet(Name = "GetHome")]
        public IActionResult GetHome([FromHeader(Name = "Accept")] string mediaType)
        {
            if (mediaType == "application/json+hateoas")
            {
                var links = new List<LinkDto>
                {
                    new LinkDto(_controllerService.UrlHelper.Link("GetHome",
                    new { }), "self", "GET"),

                    new LinkDto(_controllerService.UrlHelper.Link("GetUsers",
                    new { }), "users", "GET"),

                    new LinkDto(_controllerService.UrlHelper.Link("GetCollections",
                    new { }), "collections", "GET"),

                    new LinkDto(_controllerService.UrlHelper.Link("GetCoins",
                    new { }), "coins", "GET"),

                    new LinkDto(_controllerService.UrlHelper.Link("GetBanknotes",
                    new { }), "banknotes", "GET")
                };

                return Ok(links);
            }

            return NoContent();
        }
    }
}