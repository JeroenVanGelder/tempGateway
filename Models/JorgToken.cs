using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace exampleWebAPI.Models
{
    public class JorgToken
    {
        
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in  { get; set; }
        public string userName  { get; set; }
        
        [Key]
        public DateTime issued  { get; set; }
        public DateTime expires  { get; set; }

        public bool checkIfValid()
        {
            bool returnBool = !(expires.AddHours(2) >= DateTime.Now);
            return returnBool;
        }
    }
}