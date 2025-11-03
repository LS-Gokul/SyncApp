using System;
using System.IO;
using System.Net;
using System.Text.Json;
using Microsoft.Identity.Client;


namespace Triggers
{
    class Program
    {
        public static string isReturn, isAppCode = "AP0003", AppDataUrl = "";
        public static int iiSuccess;
        public static string Domain, Tenant, ClientId, RedirectUrl, AuthorityBase, Authority;
        public static AuthenticationResult authResult;
        public static IPublicClientApplication PublicClientApp;

        static void Main(string[] args)
        {
            ApplicationCredentails();
            /////////////////////////////////////////////////////
            PublicClientApp = PublicClientApplicationBuilder.Create(ClientId)
                            .WithRedirectUri(RedirectUrl)
                            .WithB2CAuthority(Authority)
                            .Build();
            TokenCacheHelper.EnableSerialization(PublicClientApp.UserTokenCache);
        }

        //////////////////////////////////////////////////////////////////////////////
        //Set Application Configuration Variables
        //////////////////////////////////////////////////////////////////////////////
        public static string ApplicationCredentails()
        {
            GetApplicationConfiguration(out iiSuccess, out isReturn);
            if (iiSuccess == 1)
            {
                JsonElement ljeAppConfig = createJsonElement(isReturn);

                string lsDomain, lsSignUpSignIn, lsEditProfile, lsResetPwd, lsTenant, lsAuthorityBase;

                lsDomain = ljeAppConfig[0].GetProperty("b2CIssuer").ToString();
                lsSignUpSignIn = ljeAppConfig[0].GetProperty("b2CSignInUpPolicy").ToString();
                lsEditProfile = ljeAppConfig[0].GetProperty("b2CEditPolicy").ToString();
                lsResetPwd = ljeAppConfig[0].GetProperty("b2CResetPolicy").ToString();

                lsTenant = ljeAppConfig[0].GetProperty("b2CTenantId").ToString().Replace("<Domain>", lsDomain);
                lsAuthorityBase = ljeAppConfig[0].GetProperty("b2CAuthorityBase").ToString()
                    .Replace("<Domain>", lsDomain).Replace("<Tenant>", lsTenant);

                RedirectUrl = ljeAppConfig[0].GetProperty("b2CRedirectUrl").ToString();
                ClientId = ljeAppConfig[0].GetProperty("b2CClientId").ToString();
                gsClientUrl = ljeAppConfig[0].GetProperty("clientAPIUrl").ToString();
                staging = ljeAppConfig[0].GetProperty("dBSuffix").ToString();
                Scopes = new string[] { ljeAppConfig[0].GetProperty("b2CScope").ToString() };

                Authority = $"{lsAuthorityBase}{lsSignUpSignIn}";
                AuthorityEditProfile = $"{lsAuthorityBase}{lsEditProfile}";
                AuthorityPasswordReset = $"{lsAuthorityBase}{lsResetPwd}";
                PolicySignUpSignIn = lsSignUpSignIn;
                PolicyEditProfile = lsEditProfile;
                PolicyResetPassword = lsResetPwd;

                gsContainerLocation = ljeAppConfig[0].GetProperty("containerURL").ToString();
                gsLogoContainer = ljeAppConfig[0].GetProperty("applicationContainerFolder").ToString();

                gsEmbedAutenticationMode = ljeAppConfig[0].GetProperty("embedAutenticationMode").ToString();
                gsEmbedTenantId = ljeAppConfig[0].GetProperty("embedTenantId").ToString();
                gsEmbedClientId = ljeAppConfig[0].GetProperty("embedClientId").ToString();
                gsEmbedClientSecret = ljeAppConfig[0].GetProperty("embedClientSecret").ToString();
                gsEmbedScope = ljeAppConfig[0].GetProperty("embedScope").ToString();
                gsEmbedAuthority = ljeAppConfig[0].GetProperty("embedAuthority").ToString();
                gsEmbedApiDomain = ljeAppConfig[0].GetProperty("embedApiDomain").ToString();
                gsEmbedResourceGroup = ljeAppConfig[0].GetProperty("embedResourceGroup").ToString();
                gsEmbedResource = ljeAppConfig[0].GetProperty("embedResource").ToString();
                gsEmbedResourceSubscriptionId = ljeAppConfig[0].GetProperty("embedResourceSubscriptionId").ToString();
            }
            else
            {

            }
            return "";
        }

        /////////////////////////////////////////////////////////////////////////
        //Get Application Configuration Settings
        /////////////////////////////////////////////////////////////////////////
        public static void GetApplicationConfiguration(out int aiSuccess, out string asMessage, int aiJson = 1)
        {
            asMessage = execSql("SELECT ISNULL([Embed Autentication Mode] ,'') AS embedAutenticationMode,"
                    + "     ISNULL([Embed Tenant Id], '') AS embedTenantId,ISNULL([Embed Client Id] ,'') AS embedClientId,"
                    + "     ISNULL([Embed Client Secret] ,'') AS embedClientSecret,ISNULL([Embed Scope] ,'') AS embedScope,"
                    + "     ISNULL([Embed Authority] ,'') AS embedAuthority,ISNULL([B2C Issuer] ,'') AS b2CIssuer,"
                    + "     ISNULL([B2C Tenant Id] ,'') AS b2CTenantId,ISNULL([B2C Client Id] ,'') AS b2CClientId,"
                    + "     ISNULL([B2C Client Secret] ,'') AS b2CClientSecret,ISNULL([Application Container Folder] ,'') AS applicationContainerFolder,"
                    + "     ISNULL([B2C Sign InUp Policy] ,'') AS b2CSignInUpPolicy,ISNULL([B2C Edit Policy] ,'') AS b2CEditPolicy,"
                    + "     ISNULL([B2C Reset Policy] ,'') AS b2CResetPolicy,ISNULL([B2C Redirect Url] ,'') AS b2CRedirectUrl,"
                    + "     ISNULL([B2C Authority Base] ,'') AS b2CAuthorityBase,ISNULL([B2C Scope] ,'') AS b2CScope,"
                    + "     ISNULL([Client API Url] ,'') AS clientAPIUrl,ISNULL([DB Suffix] ,'') AS dBSuffix,ISNULL([Container URL] ,'') AS containerURL,"
                    + "     ISNULL([Embed Api Domain],'') AS embedApiDomain, ISNULL([Embed Resource Group Name],'') AS embedResourceGroup,"
                    + "     ISNULL([Embed Resource Name],'') AS embedResource, ISNULL([Embed Resource Subscription Id],'') AS embedResourceSubscriptionId "
                    + " FROM[dbo].[LS_App_Setting] Where[Active] = 1 And[application_code] = '" + isAppCode + "'",
                out aiSuccess, aiJson);
        }

        private string execSql(string asSql, out int aiSuccess, int aiJson = 1,
            string asMessageOpt = "No Rows Found", int aiType = 0, int aiDBType = 0)
        {
            aiSuccess = 1;
            try
            {
                isReturn = postAPICalling(AppDataUrl + (aiDBType == 1 ? "/Master/" + aiType.ToString() :
                        (aiDBType == 2 ? "/Insert/" + aiType.ToString() : "")), "application/json",
                        "\"" + asSql.Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"", out _);
                if (aiType == 1)
                {
                    if (isReturn != "Success")
                    {
                        aiSuccess = 0;
                    }
                }
                else
                {
                    if (isReturn.Length > 9)
                    {
                        if (isReturn.Substring(0, 9) == "Failed - ")
                        {
                            aiSuccess = 0;
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                aiSuccess = 0;
                isReturn = "Failed - " + Ex.Message;
            }
            return isReturn;
        }

        public string postAPICalling(string aURL, string aContentType, string bodyContent,
            out string statusCode, string authentication = null, string Header2 = null)
        {
            string Url = String.Format(aURL);
            statusCode = "No";
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(Url);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12;
                req.KeepAlive = false;
                req.Method = "Post";

                //Adding Headers
                if (aContentType != null && aContentType != "") req.ContentType = aContentType;
                if (Header2 != null && Header2 != "") req.Headers.Add(Header2.Substring(0, Header2.IndexOf(":")).Trim(),
                            Header2.Substring(Header2.IndexOf(":") + 1).Trim());
                if (authentication != null && authentication != "") req.Headers.Add(HttpRequestHeader.Authorization, authentication);

                req.Timeout = 900000;
                string datavalue = bodyContent.Replace(Environment.NewLine, "     ");

                using (var strWr = new StreamWriter(req.GetRequestStream()))
                {
                    strWr.Write(datavalue);
                    strWr.Flush();
                    strWr.Close();

                    var respon = (HttpWebResponse)req.GetResponse();
                    statusCode = respon.StatusCode.ToString();
                    Stream stream = respon.GetResponseStream();
                    StreamReader sr = new StreamReader(stream);
                    string ApiResult = sr.ReadToEnd();
                    return ApiResult;
                }
            }
            catch (Exception e)
            {
                return "Failed - API Calling " + e.Message;
            }
        }

        //////////////////////////////////////////////////////////////////////////////
        //Creating the Parse string of the JSON Value
        //////////////////////////////////////////////////////////////////////////////
        public static JsonElement createJsonElement(string jsonString)
        {
            //jsonString = jsonString.Replace("\\", "");
            if (jsonString.Substring(0, 1) == "\"")
            {
                jsonString = jsonString.Substring(1, jsonString.Length - 2);
            }
            if (jsonString.Substring(0, 1) != "[")
            {
                jsonString = "[" + jsonString + "]";
            }
            JsonDocument jsonStrList = JsonDocument.Parse(jsonString);
            JsonElement jsonTblArray = jsonStrList.RootElement;
            return jsonTblArray;
        }
    }
}
