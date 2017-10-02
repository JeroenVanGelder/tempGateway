using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json.Linq;

namespace exampleWebAPI.Models
{
    public class Token
    {
        public Token(string asFormattedString)
        {
            var token = JObject.Parse(asFormattedString);

            AccessToken = (string) token["access_token"];
            TokenType = (string) token["token_type"];
            UserName = (string) token["userName"];
            ExpiresIn = (int) token["expires_in"];
            Issued = DateTime.Parse(token[".issued"].ToString());
            Expires = DateTime.Parse(token[".expires"].ToString());
        }

        public Token()
        {
            AccessToken = ":( :(";
            TokenType = "Bearer";
            ExpiresIn = 7198;
            UserName = "arneTest";
            Issued = DateTime.Now.AddDays(-1);
            Expires = DateTime.Now.AddDays(-1);
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string AccessToken { get; set; }
        public string TokenType { get; set; }
        public int ExpiresIn { get; set; }
        public string UserName { get; set; }
        public DateTime Issued { get; set; }
        public DateTime Expires { get; set; }

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