using EscolaInfoSys.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace EscolaInfoSys.Controllers
{
    [Route("Error")]
    public class ErrorController : Controller
    {
        [Route("404")]
        public IActionResult Error404() => View("Error404");

        [Route("500")]
        public IActionResult Error500() => View("Error500");

        [Route("403")]
        public IActionResult Error403() => View("Error403");

        [Route("GenericError")]
        public IActionResult GenericError() => View("GenericError");

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [Route("")]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

}
