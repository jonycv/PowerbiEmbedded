
namespace PowerBIEmbeddedAPI.Controllers
{
    using PowerBIEmbeddedAPI.Models;
    using PowerBIEmbeddedAPI.Services;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using System;
    using System.Text.Json;


    [ApiController]
    [Route("[controller]")]
    public class EmbedInfoController : ControllerBase
    {
        private readonly PbiEmbedService pbiEmbedService;
        private readonly IOptions<AzureAd> azureAd;
        private readonly IOptions<PowerBI> powerBI;

        /// <summary>
        /// Constructor de Embedded para Power BI
        /// </summary>
        /// <param name="pbiEmbedService">Servicio Power BI</param>
        /// <param name="azureAd">Servicio Azure AD</param>
        /// <param name="powerBI">Opciones de configuración de Power BI</param>
        public EmbedInfoController(PbiEmbedService pbiEmbedService, IOptions<AzureAd> azureAd, IOptions<PowerBI> powerBI)
        {
            this.pbiEmbedService = pbiEmbedService;
            this.azureAd = azureAd;
            this.powerBI = powerBI;
        }

        /// <summary>
        /// Devuelve el token incrustado, la URL incrustada y el vencimiento del token incrustado al cliente
        /// </summary>
        /// <returns>JSON que contiene parámetros para incrustar</returns>
        [HttpGet]
        public IActionResult GetEmbedInfo()
        {
            try
            {
                // Valida si todas las configuraciones necesarias se proporcionan en appsettings.json
                string configValidationResult = ConfigValidatorService.ValidateConfig(azureAd, powerBI);
                if (configValidationResult != null)
                {
                    HttpContext.Response.StatusCode = 400;
                    return Ok(configValidationResult);
                }

                EmbedParams embedParams = pbiEmbedService.GetEmbedParams(new Guid(powerBI.Value.WorkspaceId), new Guid(powerBI.Value.ReportId));
                return Ok(JsonSerializer.Serialize<EmbedParams>(embedParams));
            }
            catch (Exception ex)
            {
                HttpContext.Response.StatusCode = 500;
                return Ok(ex.Message + "\n\n" + ex.StackTrace);
            }
        }
    }
}
