using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace exampleWebAPI.Models
{
    public class Meting
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public DateTime Timestamp { get; set; }
        public double Temperature { get; set; }
        public double Illuminance { get; set; }
        public Weerstation Weatherstation { get; set; }
    }
}