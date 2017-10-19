﻿using System;
using System.IO;
using System.Net;
using System.Text;
using exampleWebAPI.Models;

namespace exampleWebAPI.Context
{
    public class HttpContext
    {
        private const string Url = "http://iot.jorgvisch.nl";
        private const string WeatherUrl = "/api/Weather";
        private readonly TokenContext _tokenContext = new TokenContext();

    

        public bool SendMeting(Meting value, User user)
        {
            var b = _tokenContext.CheckToken(user, this);
            return b && SendObject(value, user);
        }

        private  bool SendObject(Meting value, User user)
        {
            var json = ParseMetingToJson(value);

            var request = (HttpWebRequest) WebRequest.Create(Url + WeatherUrl);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Headers.Add("Authorization", user.Token.GetTokenString());
            request.ContentLength = json.Length;

            using (var webStream = request.GetRequestStream())
            using (var requestWriter = new StreamWriter(webStream, Encoding.ASCII))
                requestWriter.Write(json);

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

        private string ParseMetingToJson(Meting value)
        {
            return
                "{\"Weatherstation\":\"" + value.Weatherstation.Name + "\"," +
                "\"Timestamp\":\"" + value.Timestamp + "\", " +
                "\"Temperature\":" + value.Temperature + ", " +
                "\"Illuminance\": " + value.Illuminance + "}";
        }
    }
}