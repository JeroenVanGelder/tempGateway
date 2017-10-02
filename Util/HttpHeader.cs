using System;
using System.IO;
using System.Net;
using System.Text;
using exampleWebAPI.Models;

namespace exampleWebAPI.Util
{
    public class HttpHeader
    {
        private const string Url = "http://iot.jorgvisch.nl";
        private const string WeatherUrl = "/api/Weather";
        private readonly ResetToken _resetToken = new ResetToken();

  
        public bool SendMeting(Meting value, User user)
        {
            var b = CheckToken(user);
            return b && SendObject(value, user);
        }

        private bool CheckToken(User user)
        {
            if (user.Token != null && user.Token.IsValid()) return true;
            if (ResetToken(user))
                user.Token = _resetToken.GetToken(user).Result;
            return user.Token != null && user.Token.IsValid();
        }

        public bool ResetToken(User user)
        {
            return _resetToken.ResetTokenNow(user).Result;
        }

        private static bool SendObject(Meting value, User user)
        {
            var json = ParseJson(value);

            var request = (HttpWebRequest) WebRequest.Create(Url + WeatherUrl);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Headers.Add("Authorization", user.Token.GetTokenString());
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
                    if (webStream == null) return false;
                    using (var responseReader = new StreamReader(webStream))
                    {
                        var response = responseReader.ReadToEnd();
                        Console.Out.WriteLine(response);
                       
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("-----------------");
                Console.Out.WriteLine(e.Message);
                return false;
            }
        }

        private static string ParseJson(Meting value)
        {
            return
                "{\"Weatherstation\":\"" + value.Weatherstation.Name + "\"," +
                "\"Timestamp\":\"" + value.Timestamp + "\", " +
                "\"Temperature\":" + value.Temperature + ", " +
                "\"Illuminance\": " + value.Illuminance + "}";
        }
    }
}