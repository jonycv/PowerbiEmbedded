// ----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// ----------------------------------------------------------------------------

namespace PowerBi_Embedded_Net_Core_5___Angular.Services
{
    using PowerBi_Embedded_Net_Core_5___Angular.Models;
    using Microsoft.PowerBI.Api;
    using Microsoft.PowerBI.Api.Models;
    using Microsoft.Rest;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;

    public class PbiEmbedService
    {
        private readonly AadService aadService;
        private readonly string urlPowerBiServiceApiRoot = "https://api.powerbi.com";

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
            return new PowerBIClient(new Uri(urlPowerBiServiceApiRoot), tokenCredentials);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="workspaceId">Id de área de trabajo</param>
        /// <param name="reportId">Id de reporte</param>
        /// <param name="additionalDatasetId">No es necesario</param>
        /// <returns></returns>
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
          new List<EffectiveIdentity> { new EffectiveIdentity("Sales", datasetIds, rolesList) };

            embedToken = GetEmbedToken(reportId, datasetIds, workspaceId, effectiveIdentities);



            var embedReports = new List<EmbedReport>() {
                new EmbedReport
                {
                    ReportId = pbiReport.Id, ReportName = pbiReport.Name, EmbedUrl = pbiReport.EmbedUrl
                }
            };


            var embedParams = new EmbedParams
            {
                EmbedReport = embedReports,
                Type = "Report",
                EmbedToken = embedToken
            };

            return embedParams;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="reportId">Id de reorte</param>
        /// <param name="datasetIds">Ide DataSet</param>
        /// <param name="targetWorkspaceId">Destino Dataset</param>
        /// <param name="effectiveIdentities">RLS</param>
        /// <returns></returns>
        public EmbedToken GetEmbedToken(Guid reportId, IList<string> datasetIds, [Optional] Guid targetWorkspaceId, [Optional] IList<EffectiveIdentity> effectiveIdentities)
        {
            PowerBIClient pbiClient = this.GetPowerBIClient();

            var tokenRequest = new GenerateTokenRequestV2(

                reports: new List<GenerateTokenRequestV2Report>() { new GenerateTokenRequestV2Report(reportId) },

                datasets: datasetIds.Select(datasetId => new GenerateTokenRequestV2Dataset(datasetId.ToString())).ToList(),

                targetWorkspaces: targetWorkspaceId != Guid.Empty ? new List<GenerateTokenRequestV2TargetWorkspace>() { new GenerateTokenRequestV2TargetWorkspace(targetWorkspaceId) } : null,

                identities: effectiveIdentities
            );


            var embedToken = pbiClient.EmbedToken.GenerateToken(tokenRequest);

            return embedToken;
        }




    }
}
