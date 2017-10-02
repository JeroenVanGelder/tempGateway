using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace exampleWebAPI.Models
{
    public class Weerstation
    {
        public Weerstation() { }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string IpAddress { get; set; }
        public string Name { get; set; }

        public Weerstation(string ipAddress, string name)
        {
            IpAddress = ipAddress;
            Name = name;
        }
    }
}