using PowerBi_Embedded_Net_Core_5___Angular.Models;
using PowerBi_Embedded_Net_Core_5___Angular.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace PowerBi_Embedded_Net_Core_5___Angular.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PowerBIContoller : ControllerBase
    {
        private readonly PbiEmbedService pbiEmbedService;
        private readonly IOptions<AzureAd> azureAd;
        private readonly IOptions<PowerBI> powerBI;

        public PowerBIContoller(PbiEmbedService pbiEmbedService, IOptions<AzureAd> azureAd, IOptions<PowerBI> powerBI)
        {
            this.pbiEmbedService = pbiEmbedService;
            this.azureAd = azureAd;
            this.powerBI = powerBI;
        }
        /// <summary>
        /// Returns Embed token, Embed URL, and Embed token expiry to the client
        /// </summary>
        /// <returns>JSON containing parameters for embedding</returns>
        [HttpGet]
        public  IActionResult GetEmbedInfo()
        {
            try
            {
                // Validate whether all the required configurations are provided in appsettings.json
                string configValidationResult = ConfigValidatorService.ValidateConfig(azureAd, powerBI);
                if (configValidationResult != null)
                {
                    HttpContext.Response.StatusCode = 400;
                    return Ok(configValidationResult);
                }

                EmbedParams embedParams =  pbiEmbedService.GetEmbedParams(new Guid(powerBI.Value.WorkspaceId), new Guid(powerBI.Value.ReportId));
         
                return Ok(JsonSerializer.Serialize(embedParams));
            }
            catch (Exception ex)
            {
                HttpContext.Response.StatusCode = 500;
                return Ok(ex.Message + "\n\n" + ex.StackTrace);
            }
        }
    }
}
