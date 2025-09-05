using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace EscolaInfoSysApi.API
{
    [ApiController]
    [Route("api/system")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class SystemInfoController : ControllerBase
    {
        [HttpGet("info")]
        public IActionResult GetInfo()
        {
            var asm = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
            var name = asm.GetName().Name ?? "EscolaInfoSysApi";
            var version = asm.GetName().Version?.ToString() ?? "1.0.0.0";

            // Tenta ler Autor/Empresa dos atributos do assembly (se existir). Se não existir, devolve vazio.
            var companyAttr = asm.GetCustomAttribute<AssemblyCompanyAttribute>();
            var author = companyAttr?.Company ?? string.Empty;

            // Data do build aproximada pelo last write do assembly
            var location = asm.Location;
            DateTime? buildDateUtc = null;
            if (!string.IsNullOrEmpty(location) && System.IO.File.Exists(location))
            {
                buildDateUtc = System.IO.File.GetLastWriteTimeUtc(location);
            }

            return Ok(new
            {
                App = name,
                Version = version,
                Author = author,
                BuildDateUtc = buildDateUtc
            });
        }
    }
}
