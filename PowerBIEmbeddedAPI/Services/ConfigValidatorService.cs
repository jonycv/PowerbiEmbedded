
namespace PowerBIEmbeddedAPI.Services
{
    using PowerBIEmbeddedAPI.Models;
    using Microsoft.Extensions.Options;
    using System;

    public class ConfigValidatorService
    {
        /// <summary>
        /// Valida si todos los parámetros de configuración están establecidos en el archivo appsettings.json
        /// </summary>
        /// <param name="appSettings">Contiene valores de configuración de appsettings.json</param>
        /// <returns></returns>
        public static string ValidateConfig(IOptions<AzureAd> azureAd, IOptions<PowerBI> powerBI)
        {
            string message = null;
      
            bool isAuthModeServicePrincipal = azureAd.Value.AuthenticationMode.Equals("serviceprincipal", StringComparison.InvariantCultureIgnoreCase);

            if (string.IsNullOrWhiteSpace(azureAd.Value.AuthenticationMode))
            {
                message = "El modo de autenticación no está configurado en el archivo appsettings.json";
            }
            else if (string.IsNullOrWhiteSpace(azureAd.Value.AuthorityUri))
            {
                message = "La autoridad no está configurada en el archivo appsettings.json";
            }
            else if (string.IsNullOrWhiteSpace(azureAd.Value.ClientId))
            {
                message = "El ID de cliente no está configurado en el archivo appsettings.json";
            }
            else if (isAuthModeServicePrincipal && string.IsNullOrWhiteSpace(azureAd.Value.TenantId))
            {
                message = "La ID de inquilino no está establecida en el archivo appsettings.json";
            }
            else if (azureAd.Value.Scope is null || azureAd.Value.Scope.Length == 0)
            {
                message = "El alcance no está configurado en el archivo appsettings.json";
            }
            else if (string.IsNullOrWhiteSpace(powerBI.Value.WorkspaceId))
            {
                message = "El ID del espacio de trabajo no está configurado en el archivo appsettings.json";
            }
            else if (!IsValidGuid(powerBI.Value.WorkspaceId))
            {
                message = "Ingrese un guid válido para el ID del espacio de trabajo en el archivo appsettings.json";
            }
            else if (string.IsNullOrWhiteSpace(powerBI.Value.ReportId))
            {
                message = "El ID del informe no está configurado en el archivo appsettings.json";
            }
            else if (!IsValidGuid(powerBI.Value.ReportId))
            {
                message = "Ingrese una guía válida para el ID del informe en el archivo appsettings.json";
            }
           
            else if (isAuthModeServicePrincipal && string.IsNullOrWhiteSpace(azureAd.Value.ClientSecret))
            {
                message = "El secreto del cliente no está configurado en el archivo appsettings.json";
            }

            return message;
        }

        /// <summary>
        /// Comprueba si una cadena es una guía válida
        /// </summary>
        /// <param name="configParam">String value</param>
        /// <returns>Valor booleano que indica la validez del guid</returns>
        private static bool IsValidGuid(string configParam)
        {
            Guid result = Guid.Empty;
            return Guid.TryParse(configParam, out result);
        }
    }
}