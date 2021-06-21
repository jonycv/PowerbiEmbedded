
namespace PowerBIEmbeddedAPI.Services
{
    using PowerBIEmbeddedAPI.Models;
    using Microsoft.PowerBI.Api;
    using Microsoft.PowerBI.Api.Models;
    using Microsoft.Rest;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Servicio de Power BI
    /// </summary>
    public class PbiEmbedService
    {
        private readonly AadService aadService;
        private readonly string urlPowerBiServiceApiRoot  = "https://api.powerbi.com";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="aadService">Servicio de AzureAD</param>
        public PbiEmbedService(AadService aadService)
        {
            this.aadService = aadService;
        }

        /// <summary>
        /// Get Power BI client
        /// </summary>
        /// <returns>Power BI client object</returns>
        public PowerBIClient GetPowerBIClient()
        {
            var tokenCredentials = new TokenCredentials(aadService.GetAccessToken(), "Bearer");
            return new PowerBIClient(new Uri(urlPowerBiServiceApiRoot ), tokenCredentials);
        }

        /// <summary>
        /// Get parametros de embedded
        /// </summary>
        /// <param name="workspaceId">Id del área de trabajo</param>
        /// <param name="reportId">Id del reporte</param>
        /// <param name="additionalDatasetId">Ide de adataset adicional (opcional)</param>
        /// <returns>Embedded Token</returns>
        public EmbedParams GetEmbedParams(Guid workspaceId, Guid reportId, [Optional] string additionalDatasetId)
        {
            PowerBIClient pbiClient = this.GetPowerBIClient();


            var pbiReport = pbiClient.Reports.GetReportInGroup(workspaceId, reportId);

            EmbedToken embedToken;

           
                var datasetIds = new List<string>();

               
                datasetIds.Add(pbiReport.DatasetId);

              
                if (additionalDatasetId != null)
                {
                    datasetIds.Add(additionalDatasetId);
                }

                var rolesList = new List<string>();
                rolesList.Add("Sales"); 
                
    
                IList<EffectiveIdentity> effectiveIdentities =
              new List<EffectiveIdentity> { new EffectiveIdentity("USERNAME", datasetIds, rolesList) };
                embedToken = GetEmbedToken(reportId, datasetIds, workspaceId, effectiveIdentities);
            

            // Add report data for embedding
            var embedReports = new List<EmbedReport>() {
                new EmbedReport
                {
                    ReportId = pbiReport.Id, ReportName = pbiReport.Name, EmbedUrl = pbiReport.EmbedUrl
                }
            };

            // Capture embed params
            var embedParams = new EmbedParams
            {
                EmbedReport = embedReports,
                Type = "Report",
                EmbedToken = embedToken
            };

            return embedParams;
        }

       /// <summary>
       /// Obtener token para embedded
       /// </summary>
       /// <param name="reportId">Id del reporte</param>
       /// <param name="datasetIds">Id del dataset</param>
       /// <param name="targetWorkspaceId">Id del área de trabajo destino</param>
       /// <param name="effectiveIdentities"></param>
       /// <returns></returns>
        public EmbedToken GetEmbedToken(Guid reportId, IList<string> datasetIds, [Optional] Guid targetWorkspaceId, [Optional] IList<EffectiveIdentity> effectiveIdentities)
        {
            PowerBIClient pbiClient = this.GetPowerBIClient();

            // Create a request for getting Embed token 
            // This method works only with new Power BI V2 workspace experience
            var tokenRequest = new GenerateTokenRequestV2(

                reports: new List<GenerateTokenRequestV2Report>() { new GenerateTokenRequestV2Report(reportId) },

                datasets: datasetIds.Select(datasetId => new GenerateTokenRequestV2Dataset(datasetId.ToString())).ToList(),

                targetWorkspaces: targetWorkspaceId != Guid.Empty ? new List<GenerateTokenRequestV2TargetWorkspace>() { new GenerateTokenRequestV2TargetWorkspace(targetWorkspaceId) } : null,

                identities: effectiveIdentities
            );

            // Generate Embed token
            var embedToken = pbiClient.EmbedToken.GenerateToken(tokenRequest);

            return embedToken;
        }

 
    }  
}
