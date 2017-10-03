using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace exampleWebAPI.Models
{
    public class JorgToken
    {
        [Key]
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in  { get; set; }
        public string userName  { get; set; }
        
        [JsonProperty(".issued")]
        public DateTime issued  { get; set; }
        
        [JsonProperty(".expires")]
        public DateTime expires  { get; set; }

        public bool checkIfValid()
        {
            //bool returnBool = !(expires.AddHours(2) >= DateTime.Now);
            //return returnBool;
            return true;
        }
    }
}