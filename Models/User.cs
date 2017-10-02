using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace exampleWebAPI.Models
{
    public class User
    {
        public User(string email, string password)
        {
            Password = password;
            Email = email;
            Cookies = new List<MyCookie>();
        }

        public User()
        {
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // [Key]
        public string Email { get; set; }

        public string Password { get; set; }
        public Token Token { get; set; }
        public string RequestVerificationToken { get; set; }


        public ICollection<MyCookie>Cookies { get; set; }
    }
}