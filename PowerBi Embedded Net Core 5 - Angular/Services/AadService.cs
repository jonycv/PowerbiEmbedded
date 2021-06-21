// ----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// ----------------------------------------------------------------------------

namespace PowerBi_Embedded_Net_Core_5___Angular.Services
{
    using PowerBi_Embedded_Net_Core_5___Angular.Models;
    using Microsoft.Extensions.Options;
    using Microsoft.Identity.Client;
    using System;
    using System.Linq;
    using System.Security;

    public class AadService
    {
        private readonly IOptions<AzureAd> azureAd;

        public AadService(IOptions<AzureAd> azureAd)  
        {
            this.azureAd = azureAd;
        }

        /// <summary>
        /// Generar Access token
        /// </summary>
        /// <returns>AAD token</returns>
        public string GetAccessToken()
        {
            AuthenticationResult authenticationResult = null;
            if (azureAd.Value.AuthenticationMode.Equals("serviceprincipal", StringComparison.InvariantCultureIgnoreCase))
            {
                var tenantSpecificUrl = azureAd.Value.AuthorityUri.Replace("organizations", azureAd.Value.TenantId);

                IConfidentialClientApplication clientApp = ConfidentialClientApplicationBuilder
                                                                                .Create(azureAd.Value.ClientId)
                                                                                .WithClientSecret(azureAd.Value.ClientSecret)
                                                                                .WithAuthority(tenantSpecificUrl)
                                                                                .Build();
                authenticationResult = clientApp.AcquireTokenForClient(azureAd.Value.Scope).ExecuteAsync().Result;
            }

            return authenticationResult.AccessToken;
        }
    }
}
