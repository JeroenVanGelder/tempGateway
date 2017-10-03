using System;
using System.Net.Http;
using System.Text;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using exampleWebAPI.Models;
using Microsoft.Rest;
using Newtonsoft.Json;

namespace exampleWebAPI.Util
{
    public class JorgServerConnecter
    {
        private JorgToken jorgToken;
        
        private FormUrlEncodedContent wwwEncoded = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            {"grant_type","password"},
            {"username","jeroen.vangelder@student.han.nl"},
            {"password","4Cmw9Rkx!"}
        });
        
        public JorgServerConnecter(){}

        public JorgServerConnecter(JorgToken token)
        {
            this.jorgToken = token.checkIfValid() ? token : UpdateToken().Result;
        }
        
        [Obsolete("Shouldn't Back Call",false)]
        
        public async Task<JorgToken> UpdateToken()
        {
            var client = new HttpClient {BaseAddress = new Uri("http://iot.jorgvisch.nl/")};
            HttpResponseMessage response = await client.PostAsync("Token", wwwEncoded);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var responseToken = await response.Content.ReadAsStringAsync();
                jorgToken = JsonConvert.DeserializeObject<JorgToken>(responseToken);
            }

            
            return jorgToken;
        } 

        public async Task<string> postMeting(Meting meting)
        {
            //Header
            var client = new HttpClient{ BaseAddress = new Uri("http://iot.jorgvisch.nl/")};
            var authenticationToken = jorgToken.access_token;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authenticationToken);
            
            //Body
            var metinJsonString = JsonConvert.SerializeObject(meting);
            StringContent stringContent = new StringContent(metinJsonString,Encoding.UTF8,"application/json");
            
            //Post request
            HttpResponseMessage response = await client.PostAsync("api/Weather", stringContent);
            
            return response.StatusCode.ToString();
        }  
    }
}