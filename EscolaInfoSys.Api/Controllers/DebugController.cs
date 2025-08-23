using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EscolaInfoSys.Api.Controllers
{
    [ApiController]
    [Route("api/debug")]
    [AllowAnonymous]
    public class DebugController : ControllerBase
    {
        private readonly EndpointDataSource _endpoints;
        public DebugController(EndpointDataSource endpoints) => _endpoints = endpoints;

        [HttpGet("routes")]
        public IActionResult GetRoutes()
        {
            var list = _endpoints.Endpoints
                .OfType<RouteEndpoint>()
                .Select(e => new {
                    Route = e.RoutePattern.RawText,
                    Methods = string.Join(',', e.Metadata
                        .OfType<HttpMethodMetadata>()
                        .FirstOrDefault()?.HttpMethods ?? Array.Empty<string>())
                })
                .OrderBy(x => x.Route)
                .ToList();

            return Ok(list);
        }

        [HttpGet("ping")]
        public IActionResult Ping() => Ok(new { ok = true, time = DateTime.UtcNow });
    }
}
