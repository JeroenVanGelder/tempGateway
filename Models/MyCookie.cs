using System.ComponentModel.DataAnnotations;

namespace exampleWebAPI.Models
{
    public class MyCookie
    {
        [Key]
        public string Name { get; set; }

        public string Value { get; set; }
    }
}