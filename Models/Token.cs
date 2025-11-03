using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;

namespace LSSyncApp.Models
{
    public class Token
    {
        public string user_code { get; set; }
        public string LSCode { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public string Email { get; set; }
        public string Country_Calling_Code { get; set; }
        public string Mobile { get; set; }
        public string Telephone { get; set; }
        public string Approved_By { get; set; }
        public int? Isactive { get; set; }
        public int? ISAdministrator { get; set; }
        public int? Access_Sync_Application { get; set; }
        public string user_group_code { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string city_code { get; set; }
        public string state_code { get; set; }
        public string country_code { get; set; }
        public int? Postal_Code { get; set; }
        public string AAD_User_Id { get; set; }
        public string Created_User { get; set; }
        public string Modified_User { get; set; }
        public string Page_Theme { get; set; }
        public string Theme_Color { get; set; }
        public int? Access_MDM { get; set; }
        public int? IsLicensed { get; set; }
        public int? Config_Flag { get; set; }
        public int? MFA { get; set; }
        public string Default_Firm { get; set; }
        public string Zoho_Contact_ID { get; set; }
        public int? is_support_admin { get; set; }
        public int? Default_Spoc { get; set; }
        public int? isgroup { get; set; }
        public string Mobile_Key { get; set; }
    }

    public class TokenDet
    {
        public string user_details { get; set; }
    }

    public class ErrorDet
    {
        public string detail { get; set; }
    }
    public class DeserializeJWT
    {
        private readonly string CacheFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location + ".lscache.bin3";

        public void DeserializeJWTData(out int aiSuccess, out string asToken)
        {
            aiSuccess = 0;
            asToken = "";
            try
            {
                if(File.Exists(CacheFilePath))
                {
                    var token = File.ReadAllText(CacheFilePath);
                    var handler = new JwtSecurityTokenHandler();
                    var jsonToken = handler.ReadToken(token);
                    var tokenS = jsonToken as JwtSecurityToken;
                    var jti = tokenS.Claims.First(claim => claim.Type == "user_details").Value;

                    aiSuccess = 1;
                    asToken = jti;
                }
            }
            catch(Exception Ex)
            {
                asToken = Ex.Message;
            }
        }

        public void SignIn(string asSignInAPI, string asBody, RestAPI arRestAPI, out int aiSuccess)
        {
            aiSuccess = 0; 
            string lsBody = asBody;
            try
            {
                string lsReturn = arRestAPI.postAPICalling(asSignInAPI, "application/x-www-form-urlencoded", lsBody, out string aiStatusCode);
                if(aiStatusCode.ToLower() == "ok")
                {
                    TokenDet _Token = new TokenDet();
                    _Token = JsonSerializer.Deserialize<TokenDet>(lsReturn);
                    aiSuccess = 1;
                    if (File.Exists(CacheFilePath))
                        File.Delete(CacheFilePath);
                    File.WriteAllText(CacheFilePath, _Token.user_details);
                }
                else
                {
                    ErrorDet _Error = new ErrorDet();
                    _Error = JsonSerializer.Deserialize<ErrorDet>(lsReturn);
                    MessageBox.Show(_Error.detail);
                }
            }
            catch(Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }

        public void SignOut(out int aiSuccess)
        {
            aiSuccess = 0; 
            try
            {
                if (File.Exists(CacheFilePath))
                    File.Delete(CacheFilePath);
                aiSuccess = 1;
            }
            catch(Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }
    }
}
