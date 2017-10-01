using System;
using Newtonsoft.Json.Linq;

namespace exampleWebAPI.Util
{
    public  class Token
    {
        public Token(string asFormattedString)
        {
            var token = JObject.Parse(asFormattedString);

            AccessToken = (string) token["access_token"];
            TokenType = (string) token["token_type"];
            UserName = (string) token["userName"];

            //Tue, 26 Sep 2017 07:18:55 GMT
//        public string Pattern { get; } = " ddd, dd MMM yyyy HH':'mm':'ss 'GMT'";
//
//            var convertedDate = DateTime.Parse(token[".issued"].ToString());
//            Console.WriteLine("Converted {0} to {1} time {2}",
//                token[".issued"].ToString(),
//                convertedDate.Kind.ToString(),
//                convertedDate);
//

            ExpiresIn = (int) token["expires_in"];
            Issued = DateTime.Parse(token[".issued"].ToString());
            Expires = DateTime.Parse(token[".expires"].ToString());
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
            return $"{TokenType} {AccessToken}";
        }
    }
}