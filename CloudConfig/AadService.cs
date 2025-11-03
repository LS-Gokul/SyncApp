using Microsoft.Identity.Client;
using System;
using System.Linq;
using System.Security;

namespace LSSyncApp.CloudConfig
{
    public class AadService
    {
        private readonly AzureAd azureAd = new AzureAd();

        private void SetVariables(GlobalVariable gblVar)
        {
            azureAd.AuthenticationMode = gblVar.gsEmbedAutenticationMode.ToLower();
            azureAd.AuthorityUri = gblVar.gsEmbedAuthority;
            azureAd.ClientId = gblVar.gsEmbedClientId;
            azureAd.TenantId = gblVar.gsEmbedTenantId;
            azureAd.Scope = new string[] { gblVar.gsEmbedScope };
            azureAd.PbiUsername = "bi@leapsurge.in";
            azureAd.PbiPassword = "LeapSurge12#";
            azureAd.ClientSecret = gblVar.gsEmbedClientSecret;
        }

        public string GetAccessToken(GlobalVariable gblVar)
        {
            SetVariables(gblVar);
            AuthenticationResult authenticationResult = null;
            try
            {
                if (azureAd.AuthenticationMode.Equals("masteruser", StringComparison.InvariantCultureIgnoreCase))
                {
                    // Create a public client to authorize the app with the AAD app
                    IPublicClientApplication clientApp = PublicClientApplicationBuilder.Create(azureAd.ClientId).WithAuthority(azureAd.AuthorityUri).Build();
                    var userAccounts = clientApp.GetAccountsAsync().Result;
                    try
                    {
                        // Retrieve Access token from cache if available
                        authenticationResult = clientApp.AcquireTokenSilent(azureAd.Scope, userAccounts.FirstOrDefault()).ExecuteAsync().Result;
                    }
                    catch //(MsalUiRequiredException)
                    {
                        SecureString password = new SecureString();
                        foreach (var key in azureAd.PbiPassword)
                        {
                            password.AppendChar(key);
                        }
                        authenticationResult = clientApp.AcquireTokenByUsernamePassword(azureAd.Scope, azureAd.PbiUsername, password).ExecuteAsync().Result;
                    }
                }

                // Service Principal auth is the recommended by Microsoft to achieve App Owns Data Power BI embedding
                else if (azureAd.AuthenticationMode.Equals("serviceprincipal", StringComparison.InvariantCultureIgnoreCase))
                {
                    // For app only authentication, we need the specific tenant id in the authority url
                    var tenantSpecificUrl = azureAd.AuthorityUri.Replace("organizations", azureAd.TenantId);

                    // Create a confidential client to authorize the app with the AAD app
                    IConfidentialClientApplication clientApp = ConfidentialClientApplicationBuilder.Create(azureAd.ClientId)
                        .WithClientSecret(azureAd.ClientSecret).WithAuthority(tenantSpecificUrl).Build();
                    // Make a client call if Access token is not available in cache
                    authenticationResult = clientApp.AcquireTokenForClient(azureAd.Scope).ExecuteAsync().Result;
                }
            }
            catch(Exception Ex)
            {

            }
            if (authenticationResult == null)
            {
                return "";
            }
            return authenticationResult.AccessToken;
        }
    }
}
