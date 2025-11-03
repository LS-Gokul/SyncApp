using System;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;

namespace LSSyncApp
{
    
    public class RestAPI
    {
        public static int timeOut = 900000;
        /***************=========*****Function for POST Method API Calling******=========*************/
        public string postAPICalling(string aURL, string aContentType, string bodyContent,
            out string statusCode, string authentication = null, string Header2 = null)
        {
            string Url = String.Format(aURL);
            statusCode = "No";
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(Url);
                GC.Collect();
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12;
                req.KeepAlive = false;
                req.Method = "Post";
                //req.Accept = "application/json";

                //Adding Headers
                if (aContentType != null && aContentType != "") req.ContentType = aContentType;
                if (Header2 != null && Header2 != "") req.Headers.Add(Header2.Substring(0, Header2.IndexOf(":")).Trim(),
                            Header2.Substring(Header2.IndexOf(":") + 1).Trim());
                if (authentication != null && authentication != "") req.Headers.Add(HttpRequestHeader.Authorization, authentication);
                
                req.Timeout = timeOut;
                string datavalue = bodyContent.Replace(Environment.NewLine," ");

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
                    GC.Collect();
                    return ApiResult;
                }
            }
            catch (WebException ex)
            {
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                return "Failed - API Calling " + e.Message;
            }
        }

        /***************=========*****Function for GET Method API Calling******=========*************/
        public string getAPICalling(string aURL, out string statusCode, string asContentType = null, 
            string authentication = null, string Header2 = null)
        {
            string Url = String.Format(aURL);
            statusCode = "No";
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
                request.Method = "GET";
                
                //Adding Headers
                if (asContentType != null && asContentType != "") request.ContentType = asContentType;
                if (Header2 != null && Header2 != "") request.Headers.Add(Header2.Substring(0, Header2.IndexOf(":")).Trim(), 
                            Header2.Substring(Header2.IndexOf(":") + 1).Trim());
                if (authentication != null && authentication != "") request.Headers.Add(HttpRequestHeader.Authorization, authentication);
                
                request.Timeout = timeOut;
                WebResponse response = request.GetResponse();
                statusCode = ((HttpWebResponse)response).StatusCode.ToString();
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    string ApiResult = reader.ReadToEnd();
                    return ApiResult;
                }
            }
            catch (Exception e)
            {
                return "Failed - API Calling " + e.Message;
            }
        }

        public async Task<string> PostUrlAsync(string aURL, string asbody, string asContentType, 
            string asAuth = null, string asType = null)
        {
            HttpClient client = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(timeOut/1000)
            };
            HttpResponseMessage Res;
            string lsResult;
            try
            {
                asbody = asbody.Replace("\t", "&#09;");

                if (asAuth != null && asAuth != "") client.DefaultRequestHeaders.Add("Authorization", asAuth);
                StringContent TXML = new StringContent(asbody, Encoding.UTF8, asContentType);
                if(asType == "Put")
                {
                    Res = await client.PutAsync(aURL, TXML);
                }
                else
                {
                    Res = await client.PostAsync(aURL, TXML);
                }
                
                Res.EnsureSuccessStatusCode();
                var byteArray = await Res.Content.ReadAsByteArrayAsync();
                lsResult = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);
            }
            catch (Exception E)
            {
                lsResult = "Failed on API Request - " + E.Message;
            }
            return lsResult;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            GC.SuppressFinalize(this);
        }
    }
}
