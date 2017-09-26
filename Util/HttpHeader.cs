using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using exampleWebAPI.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace exampleWebAPI.Util
{
    public class HttpHeader
    {
        private const string Url = "http://iot.jorgvisch.nl/api/Weather";


        public static async Task<Token> GetToken()
        {
            var client = new HttpClient {BaseAddress = new Uri("http://iot.jorgvisch.nl")};
            var request = new HttpRequestMessage(HttpMethod.Post, "/Token");

            var keyValues = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("username", "AR.Heil@student.han.nl"),
                new KeyValuePair<string, string>("password", "Pa@76iIL1b")
            };
            request.Content = new FormUrlEncodedContent(keyValues);

            try
            {
                var response = await client.SendAsync(request);
                Console.Out.WriteLine(response.StatusCode);

                Console.Out.WriteLine(response.Content.ReadAsStringAsync().Result);

                return new Token(response.Content.ReadAsStringAsync().Result);

                //todo reset token api
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("-----------------");
                Console.Out.WriteLine(e.Message);
            }
            return new Token();
        }

        public string CreateObject(Meting value)

        {
            var token = GetToken().Result;
            if (!token.IsValid()) return "FOUT TOKEN";
            try
            {
                var json = JsonConvert.SerializeObject(value);
                var request = (HttpWebRequest) WebRequest.Create(Url);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Headers.Add("Authorization",
                    token.GetTokenString());
                request.ContentLength = json.Length;
                using (var webStream = request.GetRequestStream())
                using (var requestWriter = new StreamWriter(webStream, Encoding.ASCII))
                {
                    requestWriter.Write(json);
                }

                try
                {
                    var webResponse = request.GetResponse();
                    using (var webStream = webResponse.GetResponseStream())
                    {
                        if (webStream == null) return "FOUT";
                        using (var responseReader = new StreamReader(webStream))
                        {
                            var response = responseReader.ReadToEnd();
                            Console.Out.WriteLine(response);
                            return response;
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.Out.WriteLine("-----------------");
                    Console.Out.WriteLine(e.Message);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            return "laatste regel fout";
        }
    }

    public class Token
    {
        //Tue, 26 Sep 2017 07:18:55 GMT
//        public string Pattern { get; } = " ddd, dd MMM yyyy HH':'mm':'ss 'GMT'";

        public Token(string asFormattedString)
        {
            var token = JObject.Parse(asFormattedString);

            AccessToken = (string) token["access_token"];
            TokenType = (string) token["token_type"];
            UserName = (string) token["userName"];
//
//            var convertedDate = DateTime.Parse(token[".issued"].ToString());
//            Console.WriteLine("Converted {0} to {1} time {2}",
//                token[".issued"].ToString(),
//                convertedDate.Kind.ToString(),
//                convertedDate);
//

            ExpiresIn = (int) token["expires_in"];
            Issued = DateTime.Parse(token[".issued"].ToString());
            Expires = DateTime.Parse(token[".expires"].ToString());;
        }

        public Token()
        {
            AccessToken = ":( :(";
            TokenType = "China";
            ExpiresIn = 7198;
            UserName = "arneTest";
            Issued = DateTime.Now.AddDays(-1);
            Expires = DateTime.Now.AddDays(-1);
        }

        public string AccessToken { get; }
        public string TokenType { get; }
        private int ExpiresIn { get; }
        private string UserName { get; }
        private DateTime Issued { get; }
        private DateTime Expires { get; }

        public override string ToString()
        {
            return "" + AccessToken + " "
                   + TokenType + " "
                   + ExpiresIn + " "
                   + ExpiresIn + " "
                   + UserName + " "
                   + Issued + " "
                   + Expires;
        }

        public bool IsValid()
        {
            return Expires > DateTime.Now;
        }

        public string GetTokenString()
        {
            return "Bearer " + AccessToken;
        }
    }
}